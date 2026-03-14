using EFP48.EFCore.Data;
using EFP48.EFCore.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace EFP48.EFCore
{
    public class FirstLessonApp
    {
        public static int ShowProductCrudMenu()
        {
            Console.WriteLine(@"
1. Get all products
2. Get product by id
3. Create new product
4. Update product
5. Delete product
6. Move to categories menu
7. Exit
");
            int result;
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.WriteLine("Enter number 1-6:");
            }
            return result;
        }

        public static int ShowCategoriesMenu()
        {
            Console.WriteLine(@"
1. Get all categories
2. Add new category
3. Del category
4. Find category(By Name, By Id)
5. Update category
6. Show categories stats
7. Back to products menu
8. Exit
");
            int result;
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                Console.WriteLine("Enter number 1-6:");
            }
            return result;
        }

        // -----------------------------------------------
        // -------------- PRODUCT METHODS ----------------
        // -----------------------------------------------

        public static void printEntity<T>(List<T> values, string entityName)
        {
            if (values.Count != 0)
            {
                Console.WriteLine($"\n-------------- ${entityName} --------------");
                foreach (var item in values)
                {
                    Console.WriteLine(item);
                }
            }
            else Console.WriteLine($"No: {entityName}");
        }

        public static List<Product> getAllProducts(DataContext? dataContext, bool showDeleted = false)
        {
            if (dataContext == null)
            {
                Console.WriteLine("Error: Data context not provided");
                throw new Exception("Data Context not provided");
            }
            var products = dataContext.Products.AsQueryable();

            if (!showDeleted) products = products.Where(p => p.DeletedAt == null);

            return products.ToList();
        }

        public static Product? getProductById(DataContext? dataContext, Guid productId)
        {
            if (dataContext == null)
            {
                Console.WriteLine("Error: Data context not provided");
                throw new Exception("Data Context not provided");
            }
            // search by primary key
            //var product = dataContext.Products.Find(productId);
            // First(throws InvalidOperationExecption якщо не знаходить об'єкт), FirstOrDefault(не кидає виключень, повертає null)
            //return dataContext.Products.First(p => p.Id == productId);

            var product = dataContext.Products.FirstOrDefault(p => p.Id == productId);
            return product;
        }

        public static bool deleteProduct(DataContext? dataContext, Guid productId)
        {
            var product = getProductById(dataContext, productId);
            if (product is not null)
            {
                product.DeletedAt = DateTime.Now;
                dataContext!.SaveChanges();
                return true;
            }
            Console.WriteLine("Product not found");
            return false;
        }

        public static void createProduct(DataContext dataContext)
        {
            Console.Write("Enter product name: ");
            string? name = Console.ReadLine();

            Console.Write("Enter description: ");
            string? description = Console.ReadLine();

            Console.Write("Enter price: ");
            string? priceInput = Console.ReadLine();
            double price = 0;
            if (!double.TryParse(priceInput, out price))
            {
                Console.WriteLine("Invalid price format! Price set to 0");
            }

            Console.Write("Enter categoryId or leave it empty: ");
            string? catInput = Console.ReadLine();
            Guid? catId = null;
            if (Guid.TryParse(catInput, out Guid parsedCatId))
                catId = parsedCatId;

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = name ?? "No name",
                Description = description,
                Price = price,
                CategoryId = catId,
                CreatedAt = DateTime.Now
            };

            dataContext.Products.Add(product);
            dataContext.SaveChanges();

            Console.WriteLine("Product created successfully!");
        }

        public static void updateProduct(DataContext dataContext, Guid productId)
        {
            var product = getProductById(dataContext, productId);

            if (product is null)
            {
                Console.WriteLine("Product not found!");
                return;
            }

            Console.Write($"Enter new name (current: {product.Name}): ");
            string? newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
                product.Name = newName;

            Console.Write($"Enter new description (current: {product.Description}): ");
            string? newDesc = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newDesc))
                product.Description = newDesc;

            Console.Write($"Enter new price (current: {product.Price}): ");
            string? priceInput = Console.ReadLine();
            if (double.TryParse(priceInput, out double newPrice))
                product.Price = newPrice;

            dataContext.SaveChanges();
            Console.WriteLine("Product updated successfully!");
        }

        // ----------------------------------------------
        // ------------- CATEGORY METHODS ---------------
        // ----------------------------------------------
        public static List<Category> getAllCategories(DataContext? dataContext, bool showDeleted = false)
        {
            if (dataContext == null)
                throw new Exception("Data Context not provided");

            var categories = dataContext.Categories.AsQueryable();

            if (!showDeleted)
                categories = categories.Where(c => c.DeletedAt == null);

            return categories.ToList();
        }

        public static Category? getCategoryById(DataContext? dataContext, Guid categoryId)
        {
            if (dataContext == null)
                throw new Exception("Data Context not provided");

            return dataContext.Categories
                .FirstOrDefault(c => c.Id == categoryId && c.DeletedAt == null);
        }

        public static Category? getCategoryByName(DataContext? dataContext, string name)
        {
            if (dataContext == null)
                throw new Exception("Data Context not provided");

            return dataContext.Categories
                .FirstOrDefault(c => c.Name == name && c.DeletedAt == null);
        }

        public static void addCategory(DataContext dataContext)
        {
            Console.Write("Enter category name: ");
            string? name = Console.ReadLine();

            Console.Write("Enter description: ");
            string? description = Console.ReadLine();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = name ?? "No name",
                Description = description,
                CreatedAt = DateTime.Now
            };

            dataContext.Categories.Add(category);
            dataContext.SaveChanges();

            Console.WriteLine("Category added successfully!");
        }

        public static bool deleteCategory(DataContext dataContext, Guid categoryId)
        {
            var category = getCategoryById(dataContext, categoryId);

            if (category == null)
            {
                Console.WriteLine("Category not found");
                return false;
            }

            category.DeletedAt = DateTime.Now;
            dataContext.SaveChanges();
            return true;
        }

        public static void updateCategory(DataContext dataContext, Guid categoryId)
        {
            var category = getCategoryById(dataContext, categoryId);

            if (category == null)
            {
                Console.WriteLine("Category not found");
                return;
            }

            Console.Write("Enter new name: ");
            string? name = Console.ReadLine();

            Console.Write("Enter new description: ");
            string? description = Console.ReadLine();

            category.Name = name ?? category.Name;
            category.Description = description;

            dataContext.SaveChanges();

            Console.WriteLine("Category updated successfully!");
        }

        public static void showCategoryStats(DataContext dataContext)
        {
            var categoriesWithProducts = dataContext.Categories
                .Include(c => c.Products)
                .ToList();

            foreach (var cat in categoriesWithProducts)
            {
                Console.WriteLine($"\nCategory: {cat.Name}");

                var deletedProducts = cat.Products?.Where(p => p.DeletedAt != null).ToList();
                Console.WriteLine("Deleted products:");
                if (deletedProducts != null && deletedProducts.Count > 0)
                    foreach (var p in deletedProducts)
                        Console.WriteLine(p);
                else
                    Console.WriteLine("None");

                int count = cat.Products?.Count(p => p.DeletedAt == null) ?? 0;
                Console.WriteLine($"Products count: {count}");

                var minProduct = cat.Products?
                    .Where(p => p.DeletedAt == null)
                    .OrderBy(p => p.Price)
                    .FirstOrDefault();
                if (minProduct != null)
                    Console.WriteLine($"Cheapest product: {minProduct.Name} - {minProduct.Price}");
                else
                    Console.WriteLine("No products available");

                var prices = cat.Products?.Where(p => p.DeletedAt == null).Select(p => p.Price).ToList();
                double avg = (prices != null && prices.Count > 0) ? prices.Average() : 0;
                Console.WriteLine($"Average price: {avg:F2}");
            }
            Console.WriteLine("\nPress smth to continue");
            Console.ReadKey();
        }

        // ----------------------------------------------
        // ------------------- BRAND METHODS ---------------------
        // ----------------------------------------------


        public static void gotoBrandMenu(DataContext dataContext)
        {
            bool backToProducts = false;
            while (!backToProducts)
            {
                Console.Clear();
                int menuItem = ShowBrandsMenu();
                Guid guidParseResult;
                string userRequest;

                switch (menuItem)
                {
                    case 1:
                        Console.Clear();
                        var brands = dataContext.Brands
                            .Where(b => b.DeletedAt == null)
                            .ToList();
                        if (brands.Count > 0)
                            brands.ForEach(b => Console.WriteLine(b));
                        else
                            Console.WriteLine("No brands available rn");
                        Console.ReadKey();
                        break;

                    case 2:
                        Console.Clear();
                        addBrand(dataContext);
                        Console.ReadKey();
                        break;

                    case 3:
                        Console.Clear();
                        Console.Write("Enter brandId: ");
                        userRequest = Console.ReadLine();
                        if (Guid.TryParse(userRequest, out guidParseResult))
                            deleteBrand(dataContext, guidParseResult);
                        else
                            Console.WriteLine("Invalid Guid!");
                        Console.ReadKey();
                        break;

                    case 4:
                        Console.Clear();
                        Console.Write("Enter brandId: ");
                        userRequest = Console.ReadLine();
                        if (Guid.TryParse(userRequest, out guidParseResult))
                            updateBrand(dataContext, guidParseResult);
                        else
                            Console.WriteLine("Invalid Guid!");
                        Console.ReadKey();
                        break;

                    case 5:
                        Console.Clear();
                        ShowProductDetailedProfiles(dataContext);
                        Console.ReadKey();
                        break;

                    case 6:
                        backToProducts = true;
                        break;
                }
            }
        }

        public static int ShowBrandsMenu()
        {
            Console.WriteLine(@"
1. Show all brands
2. Add new brand
3. Delete brand
4. Update brand
5. Back
");
            int result;
            while (!int.TryParse(Console.ReadLine(), out result))
                Console.WriteLine("Enter number 1-5:");
            return result;
        }

        public static void addBrand(DataContext dataContext)
        {
            Console.Write("Enter brand name: ");
            string? name = Console.ReadLine();
            var brand = new Brand { Id = Guid.NewGuid(), Name = name ?? "No Name", CreatedAt = DateTime.Now };
            dataContext.Brands.Add(brand);
            dataContext.SaveChanges();
            Console.WriteLine("Brand added!");
        }

        public static void deleteBrand(DataContext dataContext, Guid brandId)
        {
            var brand = dataContext.Brands.Find(brandId);
            if (brand != null)
            {
                brand.DeletedAt = DateTime.Now;
                dataContext.SaveChanges();
                Console.WriteLine("Brand soft-deleted!");
            }
            else Console.WriteLine("Brand not found");
        }

        public static void updateBrand(DataContext dataContext, Guid brandId)
        {
            var brand = dataContext.Brands.Find(brandId);
            if (brand != null)
            {
                Console.Write("Enter new brand name: ");
                string? name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name))
                    brand.Name = name;
                dataContext.SaveChanges();
                Console.WriteLine("Brand updated!");
            }
            else Console.WriteLine("Brand not found");
        }

        public static void ShowProductDetailedProfiles(DataContext dataContext)
        {
            var profiles = dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Select(p => new ProductDetailedProfile
                {
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    BrandName = p.Brand.Name
                })
                .ToList();

            profiles.ForEach(p => Console.WriteLine(p));
        }

        // ----------------------------------------------
        // ------------------- MAIN ---------------------
        // ----------------------------------------------
        public static void OldMain(string[] args)
        {

            // CRUD - Create, Read, Update, Delete

            DataContext dataContext = new();

/*            var category = dataContext.Categories.Include(c => c.Products).FirstOrDefault();
            if (category is not null)
            {
                var c_products = category.Products;
                if (c_products is null || c_products.Count <= 0)
                {
                    return;
                }

                foreach (var p in c_products)
                {
                    Console.WriteLine(p);
                }
            }
            return;*/

            

        bool isExit = false;
            Guid guidParseResult;
            string userRequest;
            while (!isExit)
            {
                Console.Clear();
                int menuItem = ShowProductCrudMenu();
                switch (menuItem)
                {
                    case 1: // READ
                        Console.Clear();
                        var products = getAllProducts(dataContext);
                        printEntity<Product>(products, "Products");
                        Console.ReadKey();
                        break;
                    case 2:
                        Console.Clear();
                        Console.Write("Enter productId: ");
                        userRequest = Console.ReadLine();
                        if (!Guid.TryParse(userRequest, out guidParseResult))
                        {
                            Console.Clear();
                            Console.WriteLine("Guid format is incorrect!");
                            Console.ReadKey();
                            break;
                        }
                        var product = getProductById(dataContext, guidParseResult);
                        if (product is null)
                        {
                            Console.WriteLine("Product not found!");
                            Console.ReadKey();
                        }
                        else Console.WriteLine(product);
                        Console.ReadKey();
                        break;
                    case 3:
                        Console.Clear();
                        createProduct(dataContext);
                        Console.ReadKey();
                        break;

                    case 4:
                        Console.Clear();
                        Console.Write("Enter productId to update: ");
                        userRequest = Console.ReadLine();
                        if (!Guid.TryParse(userRequest, out guidParseResult))
                        {
                            Console.Clear();
                            Console.WriteLine("Guid format is incorrect!");
                            Console.ReadKey();
                            break;
                        }
                        updateProduct(dataContext, guidParseResult);
                        Console.ReadKey();
                        break;
                    case 5:
                        Console.Clear();
                        Console.Write("Enter productId: ");
                        userRequest = Console.ReadLine();
                        if (!Guid.TryParse(userRequest, out guidParseResult))
                        {
                            Console.Clear();
                            Console.WriteLine("Guid format is incorrect!");
                            Console.ReadKey();
                            break;
                        }
                        if (deleteProduct(dataContext, guidParseResult))
                        {
                            Console.WriteLine("Product successfuly deleted!");
                        }
                        else Console.WriteLine("Product not delete");
                        Console.ReadKey();
                        break;
                    case 6:
                        bool backToProducts = false;

                        while (!backToProducts)
                        {
                            Console.Clear();
                            int menuItem2 = ShowCategoriesMenu();

                            switch (menuItem2)
                            {
                                case 1:
                                    Console.Clear();
                                    var categories = getAllCategories(dataContext);
                                    printEntity(categories, "Categories");
                                    Console.ReadKey();
                                    break;

                                case 2:
                                    Console.Clear();
                                    addCategory(dataContext);
                                    Console.ReadKey();
                                    break;

                                case 3:
                                    Console.Clear();
                                    Console.Write("Enter categoryId: ");
                                    userRequest = Console.ReadLine();

                                    if (!Guid.TryParse(userRequest, out guidParseResult))
                                    {
                                        Console.WriteLine("Invalid Guid format!");
                                        Console.ReadKey();
                                        break;
                                    }

                                    if (deleteCategory(dataContext, guidParseResult))
                                        Console.WriteLine("Category deleted!");
                                    else
                                        Console.WriteLine("Delete failed!");

                                    Console.ReadKey();
                                    break;

                                case 4:
                                    Console.Clear();
                                    Console.WriteLine("1 - Find by Id");
                                    Console.WriteLine("2 - Find by Name");
                                    string? findOption = Console.ReadLine();

                                    if (findOption == "1")
                                    {
                                        Console.Write("Enter categoryId: ");
                                        userRequest = Console.ReadLine();

                                        if (Guid.TryParse(userRequest, out guidParseResult))
                                        {
                                            var cat = getCategoryById(dataContext, guidParseResult);
                                            Console.WriteLine(cat != null ? cat.ToString() : "Not found");
                                        }
                                    }
                                    else if (findOption == "2")
                                    {
                                        Console.Write("Enter category name: ");
                                        string? name = Console.ReadLine();

                                        var cat = getCategoryByName(dataContext, name ?? "");
                                        Console.WriteLine(cat != null ? cat.ToString() : "Not found");
                                    }

                                    Console.ReadKey();
                                    break;

                                case 5:
                                    Console.Clear();
                                    Console.Write("Enter categoryId: ");
                                    userRequest = Console.ReadLine();

                                    if (Guid.TryParse(userRequest, out guidParseResult))
                                    {
                                        updateCategory(dataContext, guidParseResult);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid Guid!");
                                    }

                                    Console.ReadKey();
                                    break;
                                case 6:
                                    Console.Clear();
                                    showCategoryStats(dataContext);
                                    break;

                                case 7:
                                    backToProducts = true;
                                    break;
                            }

                             
                        }
                        break;
                    case 7:
                        gotoBrandMenu(dataContext);
                        break;
                }

            }
            #region Lesson1
            /*
            DataContext dataContext = new();
            ShowProductCrudMenu();
            // Створюємо єкземпляр ДатаКонтексту
            // За допомогою дата-контексту у нас є можливість надсилати запити до БД


            // Створення нового продутку
            var new_product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Sony PlayStation 5 Pro",
                Price = 1500,
                CreatedAt = DateTime.Now,
            };


            // Додаємо продукт у DbSet
            dataContext.Products.Add(new_product);
            // Оновлюємо дані таблиці, зберігаємо зміни
            dataContext.SaveChanges();


            // Отримання всіх продуктів
            List<Product> products = dataContext.Products.ToList();

            Console.WriteLine("\nProducts: ");
            foreach (var p in products)
            {
                Console.WriteLine(p);
            }

            // Отримання продуктів за умовою
            products = dataContext.Products
               .Where(p => p.Price < 500)
               .ToList();

            if (products.Count > 0)
            {
                Console.WriteLine("\nProducts.Price<500: ");
                foreach (var p in products)
                {
                    Console.WriteLine(p);
                }
            }

            // Отримання продукту за ідентифікатором
            Guid searchId = Guid.Parse("f2844eab-c8c3-413c-8a55-c9fb1c95b690");

            var product = dataContext.Products.Find(searchId);
            if (product != null)
            {
                Console.WriteLine("Product: \n{0}", product);
                
                // Оновлюємо продукт
                product.Price = 340;
                // Зберігаємо зміни
                dataContext.SaveChanges();

                // Видаляємо продукт
                dataContext.Products.Remove(product);
                // Зберігаємо зміни
                dataContext.SaveChanges();
            }

            */
            #endregion

        }
    }
}