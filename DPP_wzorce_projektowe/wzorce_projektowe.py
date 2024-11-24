# Singleton + Observer
class LibraryCatalog:
    _instance = None

    def __new__(cls, *args, **kwargs):
        if not cls._instance:
            cls._instance = super().__new__(cls, *args, **kwargs)
            cls._instance.books = {}
            cls._instance.observers = {}
        return cls._instance

    def add_book(self, title, total_copies):
        if title in self.books:
            self.books[title]["total"] += total_copies
            self.books[title]["available"] += total_copies
        else:
            self.books[title] = {"available": total_copies, "total": total_copies}
        self.notify_observers(title)

    def search_book(self, title):
        return self.books.get(title, None)

    def add_observer(self, book_title, observer):
        if book_title not in self.observers:
            self.observers[book_title] = []
        self.observers[book_title].append(observer)

    def notify_observers(self, book_title):
        if book_title in self.observers:
            for observer in self.observers[book_title]:
                observer.update(f"The book '{book_title}' is now available!")

    def get_iterator(self):
        book_list = [{"title": title, "details": details} for title, details in self.books.items()]
        return BookIterator(book_list)


# Adapter
import json
import xml.etree.ElementTree as ET
import csv
from io import StringIO


class DataAdapter:
    @staticmethod
    def import_data(data, data_type):
        if data_type == "json":
            return json.loads(data)
        elif data_type == "xml":
            root = ET.fromstring(data)
            return [{"title": book.find("title").text} for book in root.findall("book")]
        elif data_type == "csv":
            reader = csv.DictReader(StringIO(data))
            return [row for row in reader]
        else:
            raise ValueError("Unsupported data type")


# Factory
class User:
    def __init__(self, name):
        self.name = name

    def get_permissions(self):
        return "Basic permissions"


class Student(User):
    def get_permissions(self):
        return "Can borrow up to 5 books"


class Teacher(User):
    def get_permissions(self):
        return "Can borrow up to 10 books"


class Librarian(User):
    def get_permissions(self):
        return "Can manage books, users, and the library system"


class Guest(User):
    def get_permissions(self):
        return "Can browse the library catalog"


class UserFactory:
    @staticmethod
    def create_user(role, name):
        if role.lower() == "student":
            return Student(name)
        elif role.lower() == "teacher":
            return Teacher(name)
        elif role.lower() == "librarian":
            return Librarian(name)
        elif role.lower() == "guest":
            return Guest(name)
        else:
            raise ValueError(f"Unknown role: {role}")


# Observer
class Observer:
    def update(self, message):
        pass


class UserObserver(Observer):
    def __init__(self, name):
        self.name = name

    def update(self, message):
        print(f"Notification for {self.name}: {message}")


# Iterator
class BookIterator:
    def __init__(self, books):
        self.books = books
        self.index = 0

    def __iter__(self):
        return self

    def __next__(self):
        if self.index < len(self.books):
            book = self.books[self.index]
            self.index += 1
            return book
        raise StopIteration


# Facade
class LibraryInterface:
    def __init__(self):
        self.catalog = LibraryCatalog()
        self.user_management = UserManagement()

    def add_book(self, title, total_copies):
        self.catalog.add_book(title, total_copies)

    def search_book(self, title):
        book = self.catalog.search_book(title)
        if book:
            return f"Book: {title}, Available: {book['available']}, Total: {book['total']}"
        return f"Book '{title}' not found."

    def add_user(self, user_id, name):
        self.user_management.add_user(user_id, name)

    def borrow_book(self, user_id, title):
        user = self.user_management.get_user(user_id)
        book = self.catalog.search_book(title)

        if not user:
            return f"User with ID {user_id} not found."
        if not book:
            return f"Book '{title}' not found in catalog."
        if book["available"] <= 0:
            return f"Book '{title}' is currently unavailable."

        self.catalog.books[title]["available"] -= 1
        self.user_management.borrow_book(user_id, title)
        return f"Book '{title}' borrowed successfully by {user['name']}."

    def return_book(self, user_id, title):
        user = self.user_management.get_user(user_id)
        book = self.catalog.search_book(title)

        if not user:
            return f"User with ID {user_id} not found."
        if not book:
            return f"Book '{title}' not found in catalog."
        if title not in user["borrowed_books"]:
            return f"User {user['name']} has not borrowed '{title}'."

        self.catalog.books[title]["available"] += 1
        self.user_management.return_book(user_id, title)
        return f"Book '{title}' returned successfully by {user['name']}."


class UserManagement:
    def __init__(self):
        self.users = {}

    def add_user(self, user_id, name):
        if user_id not in self.users:
            self.users[user_id] = {"name": name, "borrowed_books": []}

    def get_user(self, user_id):
        return self.users.get(user_id, None)

    def borrow_book(self, user_id, book_title):
        if user_id in self.users:
            self.users[user_id]["borrowed_books"].append(book_title)

    def return_book(self, user_id, book_title):
        if user_id in self.users and book_title in self.users[user_id]["borrowed_books"]:
            self.users[user_id]["borrowed_books"].remove(book_title)
if __name__ == "__main__":
    library = LibraryInterface()

    # Singleton Test
    print("=== Singleton Test ===")
    library.add_book("Dune 1", 3)
    library.add_book("1984", 5)
    print(library.search_book("Dune 1"))
    print(library.search_book("1984"))

    # Adapter Test
    print("\n=== Adapter Test ===")
    json_data = '''
    [
        {"title": "Dune", "total_copies": 5},
        {"title": "1984", "total_copies": 3},
        {"title": "Hobbit", "total_copies": 2}
    ]
    '''
    adapter = DataAdapter()
    books = adapter.import_data(json_data, "json")
    for book in books:
        library.add_book(book["title"], book["total_copies"])
    print(library.search_book("1984"))
    print(library.search_book("Dune 1"))
    print(library.search_book("Hobbit"))

    # Factory Test
    print("\n=== Factory Test ===")
    users = [
        UserFactory.create_user("student", "Jan"),
        UserFactory.create_user("teacher", "Kowalski"),
        UserFactory.create_user("librarian", "Nowak"),
        UserFactory.create_user("guest", "Guest")
    ]
    for user in users:
        print(f"{user.name}: {user.get_permissions()}")

    # Facade Test
    print("\n=== Facade Test ===")
    library.add_user(1, "Anna")
    library.add_user(2, "John")
    print(library.borrow_book(1, "Dune 1"))
    print(library.search_book("Dune 1"))
    print(library.return_book(1, "Dune 1"))

    # Observer Test
    print("\n=== Observer Test ===")
    user1 = UserObserver("Anna")
    user2 = UserObserver("John")
    library.catalog.add_observer("Hobbit", user1)
    library.catalog.add_observer("Hobbit", user2)
    library.catalog.add_book("Hobbit", 1)

    # Iterator Test
    print("\n=== Iterator Test ===")
    iterator = library.catalog.get_iterator()
    for book in iterator:
        print(f"Title: {book['title']}, Available: {book['details']['available']}, Total: {book['details']['total']}")
