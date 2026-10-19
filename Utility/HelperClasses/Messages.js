(function (window, document) {
    'use strict';

    if (window.PortalAppsMessagesV1) return;

    var hostId = 'portalapps-messages-v1';
    var styleId = 'portalapps-messages-v1-style';
    var labels = { Information: 'Bilgi', Success: 'Başarılı', Warning: 'Uyarı', Error: 'Hata' };

    function element(tag, className, text) {
        var node = document.createElement(tag);
        node.className = className;
        if (text != null) node.textContent = text;
        return node;
    }

    function ensureHost(css) {
        if (!document.getElementById(styleId)) {
            var style = document.createElement('style');
            style.id = styleId;
            style.textContent = css;
            (document.head || document.documentElement).appendChild(style);
        }
        var host = document.getElementById(hostId);
        if (!host) {
            host = element('div', 'portalapps-messages');
            host.id = hostId;
            host.setAttribute('role', 'region');
            host.setAttribute('aria-label', 'İşlem bildirimleri');
            document.body.appendChild(host);
        }
        return host;
    }

    function safeLinkUrl(url) {
        if (typeof url !== 'string' || url !== url.trim() || /[\x00-\x1f\x7f\\]/.test(url)) return false;
        var decoded;
        try { decoded = decodeURIComponent(url); } catch (error) { return false; }
        if (/[\x00-\x1f\x7f\\]/.test(decoded)) return false;
        // Server validation is authoritative; also guard direct calls to this client API.
        if (url.charAt(0) === '/' && decoded.substring(0, 2) !== '//') return true;
        var anchor = document.createElement('a');
        anchor.href = url;
        return /^https?:\/\//i.test(url) && (anchor.protocol === 'http:' || anchor.protocol === 'https:')
            && !!anchor.hostname && !anchor.username && !anchor.password;
    }

    function show(message, host) {
        if (!message || typeof message.text !== 'string' || !message.text.trim()) return;
        if (typeof message.id !== 'string' || document.getElementById(message.id)) return;
        var severity = Object.prototype.hasOwnProperty.call(labels, message.severity)
            ? message.severity : 'Information';
        var notice = element('div', 'portalapps-message portalapps-message--' + severity.toLowerCase());
        notice.id = message.id;
        var heading = element('div', 'portalapps-message__heading');
        heading.appendChild(element('strong', 'portalapps-message__title', labels[severity]));
        var close = element('button', 'portalapps-message__close', '×');
        close.type = 'button';
        close.setAttribute('aria-label', 'Bildirimi kapat');
        heading.appendChild(close);
        notice.appendChild(heading);

        // Keep controls outside the live region to avoid announcing their labels as message text.
        var content = element('div', 'portalapps-message__content');
        content.setAttribute('role', severity === 'Error' ? 'alert' : 'status');
        content.setAttribute('aria-live', severity === 'Error' ? 'assertive' : 'polite');
        content.setAttribute('aria-atomic', 'true');
        notice.appendChild(content);
        host.appendChild(notice);

        content.appendChild(element('div', 'portalapps-message__text', message.text));
        if (message.referenceCode)
            content.appendChild(element('div', 'portalapps-message__reference', 'Başvuru kodu: ' + message.referenceCode));
        if (message.link && safeLinkUrl(message.link.url)) {
            var link = element('a', 'portalapps-message__link', message.link.text);
            link.href = message.link.url;
            content.appendChild(link);
        }

        var previousFocus = document.activeElement;
        var timer = null;
        var deadline = 0;
        var remaining = Math.max(0, Number(message.autoDismissMilliseconds) || 0);
        var hovered = false;
        var focused = false;
        var closed = false;

        function dismiss(restoreFocus) {
            if (closed) return;
            closed = true;
            window.clearTimeout(timer);
            var hadFocus = notice.contains(document.activeElement);
            if (notice.parentNode) notice.parentNode.removeChild(notice);
            if (!host.firstChild && host.parentNode) host.parentNode.removeChild(host);
            if (restoreFocus && hadFocus && previousFocus && document.documentElement.contains(previousFocus))
                previousFocus.focus();
        }
        function pause() {
            if (timer !== null) {
                remaining = Math.max(0, deadline - Date.now());
                window.clearTimeout(timer);
                timer = null;
            }
        }
        function resume() {
            if (closed || hovered || focused || timer !== null || !message.autoDismissMilliseconds) return;
            deadline = Date.now() + remaining;
            timer = window.setTimeout(function () { dismiss(false); }, remaining);
        }

        close.addEventListener('click', function () { dismiss(true); });
        notice.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                event.preventDefault();
                event.stopPropagation();
                dismiss(true);
            }
        });
        notice.addEventListener('mouseenter', function () { hovered = true; pause(); });
        notice.addEventListener('mouseleave', function () { hovered = false; resume(); });
        notice.addEventListener('focusin', function () { focused = true; pause(); });
        notice.addEventListener('focusout', function (event) {
            focused = !!event.relatedTarget && notice.contains(event.relatedTarget);
            if (!focused) resume();
        });
        resume();
    }

    window.PortalAppsMessagesV1 = {
        showMany: function (messages, css) {
            if (!Array.isArray(messages) || !messages.length) return;
            var host = ensureHost(css);
            for (var index = 0; index < messages.length; index++) show(messages[index], host);
        }
    };
}(window, document));
