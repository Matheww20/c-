using System;
using System.Collections.Generic;
using System.IO;

namespace LibraryApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();
            library.LoadData(); // Загрузка данных из файлов
            Console.WriteLine("Выберите роль: 1.Библиотекарь 2.Пользователь");
            string role = Console.ReadLine();
            if (role == "1")
            {
                Librarian librarian = new Librarian("Библиотекарь");
                librarian.Start(library);
            }
            else if (role == "2")
            {
                LibraryUser user = new LibraryUser("Пользователь");
                user.Start(library);
            }
            else
            {
                Console.WriteLine("Некорректный ввод.");
            }
            library.SaveData(); // Сохранение данных в файлы
        }
    }

    abstract class User
    {
        public string Name { get; }
        public List<Book> BorrowedBooks { get; } = new List<Book>();

        protected User(string name)
        {
            Name = name;
        }
    }

    class Librarian : User
    {
        public Librarian(string name) : base(name) { }

        public void Start(Library library)
        {
            while (true)
            {
                Console.WriteLine("Функции Библиотекаря: 1.Добавить книгу 2.Удалить книгу 3.Зарегистрировать пользователя 4.Просмотреть пользователей 5.Просмотреть книги 0.Выход");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddBook(library);
                        break;
                    case "2":
                        RemoveBook(library);
                        break;
                    case "3":
                        RegisterUser(library);
                        break;
                    case "4":
                        ViewUsers(library);
                        break;
                    case "5":
                        ViewBooks(library);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Некорректный ввод.");
                        break;
                }
            }
        }

        private void AddBook(Library library)
        {
            Console.Write("Введите название книги: ");
            string title = Console.ReadLine();
            Console.Write("Введите автора книги: ");
            string author = Console.ReadLine();
            library.AddBook(new Book(title, author));
        }

        private void RemoveBook(Library library)
        {
            Console.Write("Введите название книги для удаления: ");
            string title = Console.ReadLine();
            library.RemoveBook(title);
        }

        private void RegisterUser(Library library)
        {
            Console.Write("Введите имя нового пользователя: ");
            string userName = Console.ReadLine();
            library.RegisterUser(new LibraryUser(userName));
        }

        private void ViewUsers(Library library)
        {
            Console.WriteLine("Список пользователей:");
            foreach (var user in library.Users)
            {
                Console.WriteLine(user.Name);
            }
        }

        private void ViewBooks(Library library)
        {
            Console.WriteLine("Список книг:");
            foreach (var book in library.Books)
            {
                Console.WriteLine($"{book.Title} - {book.Author} ({book.Status})");
            }
        }
    }

    class LibraryUser : User
    {
        public LibraryUser(string name) : base(name) { }

        public void Start(Library library)
        {
            while (true)
            {
                Console.WriteLine("Функции Пользователя: 1.Просмотреть книги 2.Взять книгу 3.Вернуть книгу 4.Просмотреть взятые книги 0.Выход");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ViewAvailableBooks(library);
                        break;
                    case "2":
                        BorrowBook(library);
                        break;
                    case "3":
                        ReturnBook(library);
                        break;
                    case "4":
                        ViewBorrowedBooks();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Некорректный ввод.");
                        break;
                }
            }
        }

        private void ViewAvailableBooks(Library library)
        {
            Console.WriteLine("Доступные книги:");
            foreach (var book in library.Books)
            {
                if (book.Status == "доступна")
                {
                    Console.WriteLine($"{book.Title} - {book.Author}");
                }
            }
        }

        private void BorrowBook(Library library)
        {
            Console.Write("Введите название книги, которую хотите взять: ");
            string title = Console.ReadLine();
            var book = library.GetBook(title);
            if (book != null && book.Status == "доступна")
            {
                library.BorrowBook(this, book);
            }
            else
            {
                Console.WriteLine("Книга недоступна.");
            }
        }

        private void ReturnBook(Library library)
        {
            Console.Write("Введите название книги, которую хотите вернуть: ");
            string title = Console.ReadLine();
            library.ReturnBook(this, title);
        }

        private void ViewBorrowedBooks()
        {
            Console.WriteLine("Взятые книги:");
            foreach (var book in BorrowedBooks)
            {
                Console.WriteLine($"{book.Title} - {book.Author}");
            }
        }
    }

    class Book
    {
        public string Title { get; }
        public string Author { get; }
        public string Status { get; private set; }

        public Book(string title, string author)
        {
            Title = title;
            Author = author;
            Status = "доступна";
        }

        public void Borrow() => Status = "выдана";
        public void Return() => Status = "доступна";
    }

    class Library
    {
        public List<Book> Books { get; } = new List<Book>();
        public List<LibraryUser> Users { get; } = new List<LibraryUser>();

        public void AddBook(Book book) => Books.Add(book);
        public void RemoveBook(string title) => Books.RemoveAll(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        public void RegisterUser(LibraryUser user) => Users.Add(user);
        public Book GetBook(string title) => Books.Find(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        public void BorrowBook(LibraryUser user, Book book)
        {
            book.Borrow();
            user.BorrowedBooks.Add(book);
            Console.WriteLine("Книга выдана.");
        }

        public void ReturnBook(LibraryUser user, string title)
        {
            var book = user.BorrowedBooks.Find(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (book != null)
            {
                book.Return();
                user.BorrowedBooks.Remove(book);
                Console.WriteLine("Книга возвращена.");
            }
            else
            {
                Console.WriteLine("У вас нет такой книги.");
            }
        }

        public void LoadData()
        {
            string bookFile = "books.txt";
            if (File.Exists(bookFile))
            {
                var lines = File.ReadAllLines(bookFile);
                foreach (var line in lines)
                {
                    var parts = line.Split(';');
                    if (parts.Length == 2)
                    {
                        AddBook(new Book(parts[0], parts[1]));
                    }
                }
            }
        }

        public void SaveData()
        {
            string bookFile = "books.txt";
            using (var writer = new StreamWriter(bookFile))
            {
                foreach (var book in Books)
                {
                    writer.WriteLine($"{book.Title};{book.Author}");
                }
            }
        }
    }
}