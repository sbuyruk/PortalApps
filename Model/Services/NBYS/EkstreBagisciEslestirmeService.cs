using DAO.Repositories.NBYS;
using Model.NBYS;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Model.Services.NBYS
{
    // Only identifies existing donors; never creates or changes donor records.
    public class EkstreBagisciEslestirmeService
    {
        private readonly NakitBagisciRepository repository = new NakitBagisciRepository();

        public int FindDonorId(EkstreAktarma item)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (item.NakitBagisciId > 0) return item.NakitBagisciId;

            if (item.TCKimlikNo != 0)
            {
                if (!IsValidIdentity(item.TCKimlikNo)) return 0;
                var matches = new NakitBagisci().ToList<NakitBagisci>(
                    repository.SelectByTcKimlikno(item.TCKimlikNo));
                return matches.Count == 1 && matches[0].TuzelKisi == item.TuzelKisi
                    ? matches[0].Id : 0;
            }

            string name = NormalizeName(item.Adi + " " + item.Soyadi);
            var phones = new[] { NormalizePhone(item.Telefon1), NormalizePhone(item.Telefon2) }
                .Where(phone => phone.Length > 0).Distinct().ToList();
            if (name.Length == 0 || phones.Count == 0) return 0;

            var candidates = new NakitBagisci().ToList<NakitBagisci>(repository.SelectImportCandidates(name));
            var ids = new HashSet<int>();
            foreach (string phone in phones)
            {
                var matches = candidates.Where(candidate => candidate.TuzelKisi == item.TuzelKisi
                    && NormalizeName(candidate.Adi + " " + candidate.Soyadi) == name
                    && (NormalizePhone(candidate.Telefon1) == phone
                        || NormalizePhone(candidate.Telefon2) == phone)).ToList();
                if (matches.Count != 1) return 0;
                ids.Add(matches[0].Id);
            }
            return ids.Count == 1 ? ids.First() : 0;
        }

        private static string NormalizeName(string value)
        {
            return Regex.Replace((value ?? string.Empty).Trim(), @"\s+", " ")
                .ToUpper(new CultureInfo("tr-TR"));
        }

        private static string NormalizePhone(string value)
        {
            string digits = Regex.Replace(value ?? string.Empty, @"\D", "");
            if (digits.Length == 12 && digits.StartsWith("90")) digits = digits.Substring(2);
            if (digits.Length == 11 && digits.StartsWith("0")) digits = digits.Substring(1);
            return digits.Length == 10 && !digits.All(c => c == '0') ? digits : string.Empty;
        }

        private static bool IsValidIdentity(long value)
        {
            string text = value.ToString(CultureInfo.InvariantCulture);
            if (text.Length != 11 || text[0] == '0') return false;
            int[] digits = text.Select(c => c - '0').ToArray();
            int odd = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
            int even = digits[1] + digits[3] + digits[5] + digits[7];
            return ((odd * 7 - even) % 10 + 10) % 10 == digits[9]
                && digits.Take(10).Sum() % 10 == digits[10];
        }
    }
}
