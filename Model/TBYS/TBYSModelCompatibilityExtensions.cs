using Model.Services.TBYS;
using System;
using System.Collections.Generic;
using System.Data;

namespace Model.TBYS
{
    /// <summary>
    /// Eski entity çağrılarını geçici olarak korur; bütün işlemleri Service katmanına yönlendirir.
    /// Yeni kod doğrudan ilgili Service sınıfını kullanmalıdır.
    /// </summary>
    public static class TBYSModelCompatibilityExtensions
    {
        // Kept as an empty compatibility type while downstream projects refresh references.
    }
}
