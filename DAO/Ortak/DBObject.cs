using System.Collections.Generic;

namespace DAO.Ortak
{
    public class DBObject
    {
        public string SQLString { get; set; }
        public int SQLType { get; set; }
        public int ReturnId { get; set; }
        public bool UseReturnIdAsParam { get; set; }
        public int DbObjectParamIndex { get; set; }
        //sql cünlesinde parametrenin yeri.. gelecekte kullanmak üzere planlandi
        public int SQLStringParamIndex { get; set; } = 0;
        //kaç kayit etkilendi
        public int RowsAffected { get; set; }
        //çalistiktan sonra basarili oldu mu
        public bool Success { get; set; }
        //dbobject dolduruldugunda true yapilmasi gerekir
        public bool IsFilled { get; set; }
        public string Message { get; set; }
        public HashSet<DbParam> QueryParams { get; set; }
        public HashSet<DbParam> WhereParams { get; set; }
    }
}
