using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Data.SeededData
{
    public class SeededAppData
    {
        //seeda kategorier
        public static async Task SeedCategoriesAsync(AppDbContext context)
        {
            //lägg till kategorier i listan efterhand
            var categoryName = new List<string> { "Länder", "Djur", "Frukter och grönsaker" };

            //om det inte finns en category med ett namn från listan ovan, skapa:
            foreach (var name in categoryName)
            {
                if (!context.Categories.Any(c => c.Name == name))
                {
                    context.Categories.Add(new Models.Category { Name = name });
                }
            }

            await context.SaveChangesAsync();
        }

        //seeda länder
        public static async Task SeedCountriesAsync(AppDbContext context)
        {
            //hitta kategorin
            var category = context.Categories.FirstOrDefault(c => c.Name == "Länder");

            //om kategorin inte finns, avbryt
            if (category == null)
            {
                return;
            }

            var countries = new List<string>
            {
                "Afghanistan",
"Albanien",
"Algeriet",
"Andorra",
"Angola",
"Antigua och Barbuda",
"Argentina",
"Armenien",
"Australien",
"Azerbajdzjan",
"Bahamas",
"Bahrain",
"Bangladesh",
"Barbados",
"Belarus",
"Vitryssland",
"Belgien",
"Belize",
"Benin",
"Bhutan",
"Bolivia",
"Bosnien och Hercegovina",
"Botswana",
"Brasilien",
"Brunei",
"Bulgarien",
"Burkina Faso",
"Burundi",
"Centralafrikanska republiken",
"Chile",
"Colombia",
"Comorerna",
"Demokratiska republiken Kongo",
"Danmark",
"Djibouti",
"Dominica",
"Dominikanska republiken",
"Ecuador",
"Egypten",
"El Salvador",
"Ekvatorialguinea",
"Eritrea",
"Estland",
"Eswatini",
"Swaziland",
"Etiopien",
"Fiji",
"Filippinerna",
"Finland",
"Frankrike",
"Gabon",
"Gambia",
"Georgien",
"Ghana",
"Grekland",
"Grenada",
"Guatemala",
"Guinea",
"Guinea-Bissau",
"Guyana",
"Haiti",
"Honduras",
"Indien",
"Indonesien",
"Irak",
"Iran",
"Irland",
"Island",
"Israel",
"Italien",
"Jamaica",
"Japan",
"Jordanien",
"Kambodja",
"Kamerun",
"Kanada",
"Kap Verde",
"Kazakstan",
"Kenya",
"Kina",
"Kirgizistan",
"Kiribati",
"Kolombia",
"Komorerna",
"Kongo-Brazzaville",
"Kosovo",
"Kroatien",
"Kuba",
"Kuwait",
"Laos",
"Lettland",
"Libanon",
"Liberia",
"Libyen",
"Liechtenstein",
"Litauen",
"Luxemburg",
"Madagaskar",
"Malawi",
"Malaysia",
"Maldiverna",
"Mali",
"Malta",
"Marocko",
"Marshallöarna",
"Mauretanien",
"Mauritius",
"Mexiko",
"Mikronesien",
"Moldavien",
"Monaco",
"Mongoliet",
"Montenegro",
"Mozambique",
"Myanmar",
"Burma",
"Namibia",
"Nauru",
"Nepal",
"Nicaragua",
"Nederländerna",
"New Zealand",
"Nya Zeeland",
"Niger",
"Nigeria",
"Nordkorea",
"Nordmakedonien",
"Norge",
"Oman",
"Pakistan",
"Palau",
"Panama",
"Papua Nya Guinea",
"Paraguay",
"Peru",
"Polen",
"Portugal",
"Qatar",
"Republiken Kongo",
"Rumänien",
"Rwanda",
"Ryssland",
"Saint Kitts och Nevis",
"Saint Lucia",
"Saint Vincent och Grenadinerna",
"Samoa",
"San Marino",
"Saudiarabien",
"Senegal",
"Serbien",
"Seychellerna",
"Sierra Leone",
"Singapore",
"Slovakien",
"Slovenien",
"Somalia",
"Spanien",
"Sri Lanka",
"Sudan",
"Surinam",
"Sverige",
"Schweiz",
"Syrien",
"Tadzjikistan",
"Taiwan",
"Tanzania",
"Thailand",
"Togo",
"Tonga",
"Trinidad och Tobago",
"Tunisien",
"Turkiet",
"Turkmenistan",
"Tuvalu",
"Tyskland",
"Tjeckien",
"Uganda",
"Ukraina",
"Ungern",
"Uruguay",
"Uzbekistan",
"Vanuatu",
"Vatikanstaten",
"Venezuela",
"Vietnam",
"Yemen",
"Jemen",
"Zambia",
"Zimbabwe"
            };

            foreach (var country in countries)
            {
                if (!context.Words.Any(w => w.Text.ToLower() == country.ToLower() && w.CategoryId == category.Id))
                {
                    context.Words.Add(new Models.Word
                    {
                        Text = country.ToLower(),
                        CategoryId = category.Id,
                        IsHard = false
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedAnimalsAsync(AppDbContext context)
        {
            //hitta kategorin
            var category = context.Categories.FirstOrDefault(c => c.Name == "Djur");

            //om kategorin inte finns, avbryt
            if (category == null)
            {
                return;
            }

            var animals = new List<string>
            {
                // A
                "abborre", "addax", "alpacka", "albatross", "antilop", "apa", "armadillo", "asp",
                "anka", "alligator", "axolotl", "ara", "asna", "andalusisk häst", "and",
                // B
                "bagge", "bäver", "bi", "bläckfisk", "blomfluga", "bison", "björn", "blåmes", "brungås", "berguv",
                "bisamråtta", "blåval", "buffel", "brunbjörn", "barracuda", "blåsfisk", "bergsget", "boafink",
                // C
                "chinchilla", "cikada",
                "capybara", "chamäleon",
                // D
                "dammsnäcka", "delfin", "dromedar", "dugong", "duva",
                "dingo", "domherre", "daggmask", "dovhjort", "duvhök",
                // E
                "echidna", "ejder", "ekorre", "elefant", "emu",
                "eland",
                // F
                "falk", "fasan", "fink", "fjäril", "fladdermus", "flamingo", "fluga", "flodhäst", "fisk", "fiskmås",
                "flundra", "forell", "frosk", "fransfågel",
                // G
                "gädda", "gås", "gepard", "get", "geting", "giraff", "gris", "gråsäl", "grävling", "grönfink",
                "gasell", "gorilla", "gök", "gräshoppa", "groda", "grävand", "gråhäger", "gnagare",
                // H
                "hackspett", "hamster", "havsörn", "höna", "haj", "hermelin", "hund", "häst", "hyena", "hörnuggla",
                "hare", "huggorm", "hummer", "häger", "humla", "hjort", "husdjur", "hök",
                // I
                "ibis", "igelkott", "iller", "isfågel", "isbjörn",
                "impala",
                // J
                "järv", "jaguar", "jorduggla",
                "jak", "jordekorre",
                // K
                "kaja", "kamel", "kalkon", "kanin", "känguru", "kanadagås", "katt", "kolibri", "koltrast",
                "kobra", "ko", "kopparödla", "korp", "krabba", "krokodil", "kungsörn",
                "koala", "kråka", "kräfta", "klapptrut", "kattfisk",
                // L
                "lama", "lejon", "lemmel", "leopard", "lodjur", "lax", "lärka", "långörad uggla", "lunnefågel",
                "lemur", "lagerödla", "loppa", "lysmask",
                // M
                "makrill", "mal", "manet", "mård", "mink", "mus", "myskoxe", "mygg", "myra", "mullvad", "mås",
                "marulk", "mangust", "markatta", "mantarocka", "murmeldjur",
                // N
                "noshörning", "näbbmus", "näktergal", "näbbval", "nötskrika",
                "narval", "nötväcka", "nandu", "nilkrokodil",
                // O
                "oxe", "orangutang", "orm", "ormvråk",
                "ozelot", "okapi",
                // P
                "panda", "papegoja", "pelikan", "pingvin", "piraya", "prärievarg", "påfågel", "puma",
                "pyton", "padda", "pilgrimsfalk", "ponny", "papegojfisk",
                // R
                "rådjur", "råtta", "ren", "ripa", "rocka", "rödhake", "regnbågsöring",
                "räv", "rödräv", "rödspätta", "röding", "rapphöna", "rödstjärt",
                // S
                "säl", "sandödla", "skarv", "skata", "sköldpadda", "snok", "sengångare", "sjöhäst", "sjölejon",
                "svala", "spindel", "spindelkrabba", "struts", "stork", "svan", "surikat",
                "sill", "snigel", "sparv", "stare", "stenbock", "sardin", "sjöstjärna", "skorpion", "salamander",
                "silkesmask", "sidensvans", "sillgrissla", "svan",
                // T
                "talgoxe", "tallbit", "tapir", "tasmansk djävul", "tiger", "tigerhaj", "tjäder",
                "tonfisk", "torsk", "trana", "trollslända", "tukan", "tumlare", "tupp",
                "tornfalk", "tusenfoting", "tornuggla", "trädkrypare",
                // U
                "uggla", "undulat", "uer", "utter",
                // V
                "varg", "val", "valross", "varan", "vessla", "vildand", "vildsvin",
                "vipa", "vombat", "vråk", "vattenbuffel", "vaktel", "vattensalamander",
                // Z
                "zebra",
                // Å
                "ål", "åsna",
                // Ä
                "älg", "ängspiplärka", "ängshök",
                "ärtsångare",
                // Ö
                "örn", "ödla", "öring", "ökenräv",
                "örnuggla",
            };

            foreach (var animal in animals)
            {
                if (!context.Words.Any(w => w.Text.ToLower() == animal.ToLower() && w.CategoryId == category.Id))
                {
                    context.Words.Add(new Models.Word
                    {
                        Text = animal.ToLower(),
                        CategoryId = category.Id,
                        IsHard = false
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedFruitsAndVegetablesAsync(AppDbContext context)
        {
            var category = context.Categories.FirstOrDefault(c => c.Name == "Frukter och grönsaker");

            if(category == null)
            {
                return;
            }

            var fruitsAndVegetables = new List<string>
            {
                // A
                "ananas", "apelsin", "avokado", "aubergine", "aprikos", "artisjocka",
                // B
                "banan", "björnbär", "bladspenat", "blomkål", "blåbär", "bondböna",
                "broccolini", "broccoli", "brysselkål", "brytböna", "butternutpumpa",
                "belugalinser", "bataat",
                // C
                "cantaloupe", "chili", "citron", "clementin",
                "cashewnöt", "chilifrukt",
                // D
                "druva", "durian",
                "dadeltomat", "dill", "dadel",
                // E
                "endiv", "eklöksallad", "edamamböna", "elderflower",
                // F
                "fikon", "fänkål", "fläderbär",
                "frisée", "fyrkantsmelon",
                // G
                "grönkål", "guava", "gurka",
                "grapefrukt", "granatäpple", "grönböna", "gullök", "gul paprika",
                // H
                "hallon", "honungsmelon",
                "habanero", "haricots verts", "hjortron", "havtorn",
                // I
                "ingefära", "isbergssallad",
                // J
                "jalapeño", "jordärtskocka", "jordgubbe",
                "jackfrukt", "junibär",
                // K
                "karambola", "kikärta", "kiwano", "kiwi", "knipplök", "kokosnöt",
                "kål", "kålrabbi", "körsbär",
                "kumquat", "kronärtskocka", "kantarell", "koriander", "krasse", "krusbär",
                // L
                "lime", "litchi",
                "lingon", "lök", "limeblad", "lagerblad", "linser",
                // M
                "majs", "majrova", "mandarin", "mango", "mangostan", "mangold",
                "maracuja", "mirabel", "morot", "mullbär",
                "melon", "mynta",
                // N
                "nektarin",
                "nypon", "nässla", "navelapelsin", "nutritionsärt",
                // O
                "oliv", "okra", "oregano",
                // P
                "pak choi", "palsternacka", "papaya", "paprika", "passionsfrukt", "pastinacka",
                "pepparrot", "persika", "physalis", "plommon", "pomelo", "potatis", "pumpa",
                "purjolök", "päron",
                "persilja", "pitahaya",
                // R
                "rabarber", "ramslök", "rambutan", "raps", "rädisa", "rödbeta",
                "rödkål", "rödlök", "rotselleri", "ruccola",
                "rosenkål", "rönnbär", "rödvinbär",
                // S
                "sallad", "satsuma", "schalottenlök", "snöärta", "sockerärta", "sojaböna",
                "sparris", "spenat", "stjärnfrukt", "svartrot", "svartkål", "squash", "sötpotatis",
                "selleri", "svartvinbär", "smultron",
                // T
                "tomat", "tomatillo",
                "tryffel", "timjan", "tamarind", "turnips",
                // U
                "umeboshi", "ugli",
                // V
                "vattenmelon", "vaxböna", "vindruva", "vitkål", "vitlök",
                "valnöt", "vanilj",
                // Y
                "yuzu",
                // Z
                "zucchini",
                // Ä
                "äpple", "ärta",
                "ättiksgurka",
                // Ö
                "örtgurka", "ölkrasse",
            };

            foreach (var item in fruitsAndVegetables)
            {
                if (!context.Words.Any(w => w.Text.ToLower() == item.ToLower() && w.CategoryId == category.Id))
                {
                    context.Words.Add(new Models.Word
                    {
                        Text = item.ToLower(),
                        CategoryId = category.Id,
                        IsHard = false
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
