using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Data.SeededData
{
    public class SeededAppData
    {
        public static async Task SeedCategoriesAsync(AppDbContext context)
        {
            var categoryName = new List<string> { "Countries", "Animals", "Fruits and Vegetables" };

            foreach (var name in categoryName)
            {
                if (!context.Categories.Any(c => c.Name == name))
                {
                    context.Categories.Add(new Models.Category { Name = name });
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedCountriesAsync(AppDbContext context)
        {
            var category = context.Categories.FirstOrDefault(c => c.Name == "Countries");

            if (category == null)
            {
                return;
            }

            var countries = new List<string>
            {
                "Afghanistan",
                "Albania",
                "Algeria",
                "Andorra",
                "Angola",
                "Antigua and Barbuda",
                "Argentina",
                "Armenia",
                "Australia",
                "Azerbaijan",
                "Bahamas",
                "Bahrain",
                "Bangladesh",
                "Barbados",
                "Belarus",
                "Belgium",
                "Belize",
                "Benin",
                "Bhutan",
                "Bolivia",
                "Bosnia and Herzegovina",
                "Botswana",
                "Brazil",
                "Brunei",
                "Bulgaria",
                "Burkina Faso",
                "Burundi",
                "Central African Republic",
                "Chile",
                "Colombia",
                "Comoros",
                "Democratic Republic of the Congo",
                "Denmark",
                "Djibouti",
                "Dominica",
                "Dominican Republic",
                "Ecuador",
                "Egypt",
                "El Salvador",
                "Equatorial Guinea",
                "Eritrea",
                "Estonia",
                "Eswatini",
                "Ethiopia",
                "Fiji",
                "Philippines",
                "Finland",
                "France",
                "Gabon",
                "Gambia",
                "Georgia",
                "Ghana",
                "Greece",
                "Grenada",
                "Guatemala",
                "Guinea",
                "Guinea-Bissau",
                "Guyana",
                "Haiti",
                "Honduras",
                "India",
                "Indonesia",
                "Iraq",
                "Iran",
                "Ireland",
                "Iceland",
                "Israel",
                "Italy",
                "Jamaica",
                "Japan",
                "Jordan",
                "Cambodia",
                "Cameroon",
                "Canada",
                "Cape Verde",
                "Kazakhstan",
                "Kenya",
                "China",
                "Kyrgyzstan",
                "Kiribati",
                "Republic of the Congo",
                "Kosovo",
                "Croatia",
                "Cuba",
                "Kuwait",
                "Laos",
                "Latvia",
                "Lebanon",
                "Liberia",
                "Libya",
                "Liechtenstein",
                "Lithuania",
                "Luxembourg",
                "Madagascar",
                "Malawi",
                "Malaysia",
                "Maldives",
                "Mali",
                "Malta",
                "Morocco",
                "Marshall Islands",
                "Mauritania",
                "Mauritius",
                "Mexico",
                "Micronesia",
                "Moldova",
                "Monaco",
                "Mongolia",
                "Montenegro",
                "Mozambique",
                "Myanmar",
                "Namibia",
                "Nauru",
                "Nepal",
                "Nicaragua",
                "Netherlands",
                "New Zealand",
                "Niger",
                "Nigeria",
                "North Korea",
                "North Macedonia",
                "Norway",
                "Oman",
                "Pakistan",
                "Palau",
                "Panama",
                "Papua New Guinea",
                "Paraguay",
                "Peru",
                "Poland",
                "Portugal",
                "Qatar",
                "Romania",
                "Rwanda",
                "Russia",
                "Saint Kitts and Nevis",
                "Saint Lucia",
                "Saint Vincent and the Grenadines",
                "Samoa",
                "San Marino",
                "Saudi Arabia",
                "Senegal",
                "Serbia",
                "Seychelles",
                "Sierra Leone",
                "Singapore",
                "Slovakia",
                "Slovenia",
                "Somalia",
                "Spain",
                "Sri Lanka",
                "Sudan",
                "Suriname",
                "Sweden",
                "Switzerland",
                "Syria",
                "Tajikistan",
                "Taiwan",
                "Tanzania",
                "Thailand",
                "Togo",
                "Tonga",
                "Trinidad and Tobago",
                "Tunisia",
                "Turkey",
                "Turkmenistan",
                "Tuvalu",
                "Germany",
                "Czech Republic",
                "Uganda",
                "Ukraine",
                "Hungary",
                "Uruguay",
                "Uzbekistan",
                "Vanuatu",
                "Vatican City",
                "Venezuela",
                "Vietnam",
                "Yemen",
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
            var category = context.Categories.FirstOrDefault(c => c.Name == "Animals");

            if (category == null)
            {
                return;
            }

            var animals = new List<string>
            {
                // A
                "perch", "addax", "alpaca", "albatross", "antelope", "monkey", "armadillo", "asp",
                "duck", "alligator", "axolotl", "macaw", "donkey", "andalusian horse",
                // B
                "ram", "beaver", "bee", "octopus", "hoverfly", "bison", "bear", "blue tit", "bean goose", "eagle owl",
                "muskrat", "blue whale", "buffalo", "brown bear", "barracuda", "pufferfish", "mountain goat", "java sparrow",
                // C
                "chinchilla", "cicada",
                "capybara", "chameleon",
                // D
                "pond snail", "dolphin", "dromedary", "dugong", "pigeon",
                "dingo", "bullfinch", "earthworm", "fallow deer", "goshawk",
                // E
                "echidna", "eider", "squirrel", "elephant", "emu",
                "eland",
                // F
                "falcon", "pheasant", "finch", "butterfly", "bat", "flamingo", "fly", "hippopotamus", "fish", "herring gull",
                "flounder", "trout", "frog", "jacana",
                // G
                "pike", "goose", "cheetah", "goat", "wasp", "giraffe", "pig", "grey seal", "badger", "greenfinch",
                "gazelle", "gorilla", "cuckoo", "grasshopper", "shelduck", "grey heron", "rodent",
                // H
                "woodpecker", "hamster", "white-tailed eagle", "hen", "shark", "stoat", "dog", "horse", "hyena", "eared owl",
                "hare", "adder", "lobster", "heron", "bumblebee", "deer", "hawk",
                // I
                "ibis", "hedgehog", "polecat", "kingfisher", "polar bear",
                "impala",
                // J
                "wolverine", "jaguar", "burrowing owl",
                "yak", "ground squirrel",
                // K
                "jackdaw", "camel", "turkey", "rabbit", "kangaroo", "canada goose", "cat", "hummingbird", "blackbird",
                "cobra", "cow", "raven", "crab", "crocodile", "golden eagle",
                "koala", "crow", "crayfish", "catfish",
                // L
                "llama", "lion", "lemming", "leopard", "lynx", "salmon", "lark", "long-eared owl", "puffin",
                "lemur", "flea", "glowworm",
                // M
                "mackerel", "moth", "jellyfish", "marten", "mink", "mouse", "musk ox", "mosquito", "ant", "mole", "seagull",
                "anglerfish", "mongoose", "vervet monkey", "manta ray", "marmot",
                // N
                "rhinoceros", "shrew", "nightingale", "narwhal", "jay",
                "nuthatch", "rhea", "nile crocodile",
                // O
                "ox", "orangutan", "snake", "buzzard",
                "ocelot", "okapi",
                // P
                "panda", "parrot", "pelican", "penguin", "piranha", "coyote", "peacock", "puma",
                "python", "toad", "peregrine falcon", "pony", "parrotfish",
                // R
                "roe deer", "rat", "reindeer", "ptarmigan", "ray", "robin", "rainbow trout",
                "fox", "red fox", "plaice", "arctic char", "partridge", "redstart",
                // S
                "seal", "sand lizard", "cormorant", "magpie", "tortoise", "grass snake", "sloth", "seahorse", "sea lion",
                "swallow", "spider", "spider crab", "ostrich", "stork", "swan", "meerkat",
                "herring", "snail", "sparrow", "starling", "ibex", "sardine", "starfish", "scorpion", "salamander",
                "silkworm", "waxwing", "razorbill",
                // T
                "great tit", "crossbill", "tapir", "tasmanian devil", "tiger", "tiger shark", "capercaillie",
                "tuna", "cod", "crane", "dragonfly", "toucan", "porpoise", "rooster",
                "kestrel", "centipede", "barn owl", "treecreeper",
                // U
                "owl", "budgerigar", "redfish", "otter",
                // V
                "wolf", "whale", "walrus", "monitor lizard", "weasel", "mallard", "wild boar",
                "lapwing", "wombat", "water buffalo", "quail", "newt",
                // Z
                "zebra",
                // E
                "eel",
                // M
                "moose", "meadow pipit", "hen harrier",
                "lesser whitethroat",
                // E
                "eagle", "lizard", "fennec fox",
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
            var category = context.Categories.FirstOrDefault(c => c.Name == "Fruits and Vegetables");

            if (category == null)
            {
                return;
            }

            var fruitsAndVegetables = new List<string>
            {
                // A
                "pineapple", "orange", "avocado", "eggplant", "apricot", "artichoke",
                // B
                "banana", "blackberry", "spinach", "cauliflower", "blueberry", "broad bean",
                "broccolini", "broccoli", "brussels sprouts", "green bean", "butternut squash",
                "beluga lentils", "bataat",
                // C
                "cantaloupe", "chili", "lemon", "clementine",
                "cashew nut", "chili pepper",
                // D
                "grape", "durian",
                "cherry tomato", "dill", "date",
                // E
                "endive", "oak leaf lettuce", "edamame", "elderflower",
                // F
                "fig", "fennel", "elderberry",
                "frisée", "square melon",
                // G
                "kale", "guava", "cucumber",
                "grapefruit", "pomegranate", "yellow onion", "yellow bell pepper",
                // H
                "raspberry", "honeydew melon",
                "habanero", "haricots verts", "cloudberry", "sea buckthorn",
                // I
                "ginger", "iceberg lettuce",
                // J
                "jalapeño", "jerusalem artichoke", "strawberry",
                "jackfruit", "juneberry",
                // K
                "carambola", "chickpea", "kiwano", "kiwi", "spring onion", "coconut",
                "cabbage", "kohlrabi", "cherry",
                "kumquat", "chanterelle", "coriander", "cress", "gooseberry",
                // L
                "lime", "lychee",
                "lingonberry", "onion", "lime leaf", "bay leaf", "lentils",
                // M
                "corn", "turnip", "mandarin", "mango", "mangosteen", "swiss chard",
                "passion fruit", "mirabelle plum", "carrot", "mulberry",
                "melon", "mint",
                // N
                "nectarine",
                "rosehip", "nettle", "navel orange",
                // O
                "olive", "okra", "oregano",
                // P
                "bok choy", "parsnip", "papaya", "bell pepper", "passionfruit", "horseradish", "peach",
                "physalis", "plum", "pomelo", "potato", "pumpkin",
                "leek", "pear",
                "parsley", "dragon fruit",
                // R
                "rhubarb", "wild garlic", "rambutan", "rapeseed", "radish", "beetroot",
                "red cabbage", "red onion", "celeriac", "arugula",
                "rowanberry", "red currant",
                // S
                "lettuce", "satsuma", "shallot", "snow pea", "sugar snap pea", "soybean",
                "asparagus", "star fruit", "salsify", "cavolo nero", "squash", "sweet potato",
                "celery", "blackcurrant", "wild strawberry",
                // T
                "tomato", "tomatillo",
                "truffle", "thyme", "tamarind",
                // U
                "umeboshi", "ugli fruit",
                // V
                "watermelon", "wax bean", "white cabbage", "garlic",
                "walnut", "vanilla",
                // Y
                "yuzu",
                // Z
                "zucchini",
                // A
                "apple", "pea",
                "pickled cucumber",
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
