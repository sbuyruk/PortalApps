namespace DAO.Ortak
{
    public class DBObject
    {
        public string SQLString { get; set; }
        public int SQLType { get; set; }
        public int ReturnId { get; set; }
        public bool UseReturnIdAsParam { get; set; }
        public int DbObjectParamIndex { get; set; }
        //sql cünlesinde parametrenin yeri.. gelecekte kullanmak üzere planlandı
        public int SQLStringParamIndex { get; set; }
        //kaç kayıt etkilendi
        public int RowsAffected { get; set; }
        //çalıştıktan sonra başarılı oldu mu
        public bool Success { get; set; }
        //dbobject doldurulduğunda true yapılması gerekir
        public bool IsFilled { get; set; }
        public string Message { get; set; }
    }
}
