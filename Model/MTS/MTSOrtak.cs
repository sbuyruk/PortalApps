using Utility.ProjeGlobal;

namespace Model.Ortak
{
    public class MTSOrtak
    {
        public static string ParseFaaliyetAmaci(string amac)
        {
            string amacStr = string.Empty;
            switch (amac)
            {
                case ProjeConstants.FAALIYET_AMACI_DAVET_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_DAVET;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_IZIN_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_IZIN;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_OZELCALISMA_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_OZELCALISMA;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_RESMITATIL_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_RESMITATIL;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_TOPLANTI_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_TOPLANTI;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_YILDONUMU_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_YILDONUMU;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_ZIYARET_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_ZIYARET;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_GORUSME_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_GORUSME;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_SEYAHAT_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_SEYAHAT;
                        break;
                    }
                case ProjeConstants.FAALIYET_AMACI_BILGI_INT:
                    {
                        amacStr = ProjeConstants.FAALIYET_AMACI_BILGI;
                        break;
                    }
                default:
                    break;
            }
            return amacStr;
        }

        public static string ParseFaaliyetDurumu(int durum)
        {
            string durumStr = string.Empty;
            switch (durum)
            {
                case ProjeConstants.FAALIYET_DURUMU_PLANLANDI_INT:
                    {
                        durumStr = ProjeConstants.FAALIYET_DURUMU_PLANLANDI;
                        break;
                    }
                case ProjeConstants.FAALIYET_DURUMU_ONAYLANDI_INT:
                    {
                        durumStr = ProjeConstants.FAALIYET_DURUMU_ONAYLANDI;
                        break;
                    }
                case ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI_INT:
                    {
                        durumStr = ProjeConstants.FAALIYET_DURUMU_IPTALEDILDI;
                        break;
                    }
                default:
                    break;
            }
            return durumStr;
        }
    }
}
