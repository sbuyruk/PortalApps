# Copilot Instructions

## Project Guidelines
- Project standard: all source files must use an encoding that supports Turkish characters; files that do not support Turkish must be corrected.
- Never modify, delete, create, or edit files with .ascx.g.cs extension under any circumstances — this is a permanent, standing rule, not limited to a single conversation. These are auto-generated SharePoint/ASP.NET WebForms designer files handled exclusively via Visual Studio (e.g., Run Custom Tool). When these files are missing or stale, handle them in Visual Studio. Any changes to these files will be lost or cause compilation errors.

## Interaction Guidelines
- Provide short, direct, and non-verbose answers.
- Make only the requested changes; do not make extra changes, and ask if something is really necessary.
- Complete tasks in one attempt; if information is lacking, ask a maximum of 3 clear questions; otherwise, make assumptions and proceed.
- When adding code changes, always use the `replace_string_in_file` or `multi_replace_string_in_file` tools to apply edits directly to files rather than presenting code blocks in the chat response for the user to apply manually.

## Code Guidelines
- Use `TryConvertTurkishDecimal()` method instead of `ConvertToDecimal()` in newly written code. If a code change requires testing, evaluate the conversion from `ConvertToDecimal` to `TryConvertTurkishDecimal`.

### Transaction bütünlüğü
- `SqlTransactionContext transaction` parametresi alan bir metot, yaptığı tüm veritabanı `SELECT`, `INSERT`, `UPDATE` ve `DELETE` işlemlerini aynı transaction üzerinden çalıştırmalıdır.
- Bu metotların içinden transaction parametresi olmayan repository/service metotları, `new DbClass()` veya ayrı bağlantı açan başka bir DB erişimi çağrılamaz.
- Transaction parametresi nullable olan legacy yardımcı metotlarda yalnızca `transaction == null` dalı eski bağlantıyı kullanabilir; transaction verilmiş dalda tüm iç çağrılar transaction-aware overload olmalıdır.
- Olay/audit kayıtları da aynı transaction üzerinden yazılmalıdır; `NOLOCK` bu kuralı aşmak için kullanılamaz.
