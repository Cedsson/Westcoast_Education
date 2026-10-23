using WestcoastEducation.Models;
using WestcoastEducation.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var registry = new EducationRegistry();
SeedTestData(registry);

var running = true;
while (running)
{
    PrintMenu();
    var choice = ReadLine().Trim();
    Console.WriteLine();

    try
    {
        switch (choice)
        {
            case "1":
                registry.ListCourses();
                break;
            case "2":
                ListCoursesByType(registry);
                break;
            case "3":
                registry.ListStudents();
                break;
            case "4":
                AddCourse(registry);
                break;
            case "5":
                AddStudent(registry);
                break;
            case "6":
                AddTeacher(registry);
                break;
            case "7":
                BookStudent(registry);
                break;
            case "0":
                running = false;
                Console.WriteLine("Hejdå!");
                break;
            default:
                Console.WriteLine("Ogiltigt val, försök igen.");
                break;
        }
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        // Valideringsfel från modellerna visas som vanliga felmeddelanden.
        Console.WriteLine($"Fel: {ex.Message}");
    }
}

static void PrintMenu()
{
    Console.WriteLine();
    Console.WriteLine("=== WestCoast Education ===");
    Console.WriteLine("1. Lista kurser");
    Console.WriteLine("2. Lista klassrumskurser / distanskurser");
    Console.WriteLine("3. Lista studerande");
    Console.WriteLine("4. Lägg till kurs");
    Console.WriteLine("5. Lägg till studerande");
    Console.WriteLine("6. Lägg till lärare");
    Console.WriteLine("7. Boka student på kurs");
    Console.WriteLine("0. Avsluta");
    Console.Write("Val: ");
}

static void ListCoursesByType(EducationRegistry registry)
{
    var type = ReadCourseType();
    registry.ListCourses(type);
}

static void AddCourse(EducationRegistry registry)
{
    var number = ReadText("Kursnummer: ");
    var title = ReadText("Titel: ");
    var start = ReadDate("Startdatum (åååå-mm-dd): ");
    var end = ReadDate("Slutdatum (åååå-mm-dd): ");
    var type = ReadCourseType();

    var course = new Course(number, title, start, end, type);

    Console.Write("Ansvarig lärares personnummer (lämna tomt för ingen): ");
    var teacherNumber = ReadLine().Trim();
    if (!string.IsNullOrEmpty(teacherNumber))
    {
        var teacher = registry.FindTeacher(teacherNumber);
        if (teacher == null)
        {
            Console.WriteLine("Läraren hittades inte, kursen läggs till utan lärare.");
        }
        else
        {
            course.AssignTeacher(teacher);
        }
    }

    registry.AddCourse(course);
    Console.WriteLine($"Kursen lades till: {course}");
}

static void AddStudent(EducationRegistry registry)
{
    var student = new Student(
        ReadText("Förnamn: "), ReadText("Efternamn: "), ReadText("Telefon: "),
        ReadText("Personnummer: "), ReadText("Adress: "), ReadText("Postnummer: "), ReadText("Ort: "));

    registry.AddStudent(student);
    Console.WriteLine($"Studenten lades till: {student}");
}

static void AddTeacher(EducationRegistry registry)
{
    var teacher = new Teacher(
        ReadText("Förnamn: "), ReadText("Efternamn: "), ReadText("Telefon: "),
        ReadText("Personnummer: "), ReadText("Adress: "), ReadText("Postnummer: "), ReadText("Ort: "),
        ReadText("Kunskapsområden (kommaseparerade): ").Split(','));

    registry.AddTeacher(teacher);
    Console.WriteLine($"Läraren lades till: {teacher}");
}

static void BookStudent(EducationRegistry registry)
{
    registry.ListCourses();
    var courseNumber = ReadText("Kursnummer: ");
    registry.ListStudents();
    var personalNumber = ReadText("Studentens personnummer: ");

    registry.BookStudent(courseNumber, personalNumber);
    Console.WriteLine("Studenten är bokad på kursen.");
}

// --- Hjälpmetoder för inmatning ---

// Gemensam inläsning: avslutar programmet om indataströmmen tar slut.
static string ReadLine()
{
    var line = Console.ReadLine();
    if (line == null)
        Environment.Exit(0);
    return line;
}

static string ReadText(string prompt)
{
    Console.Write(prompt);
    return ReadLine();
}

static DateTime ReadDate(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (DateTime.TryParse(ReadLine(), out var date))
            return date;
        Console.WriteLine("Ogiltigt datum, försök igen.");
    }
}

static CourseType ReadCourseType()
{
    while (true)
    {
        Console.Write("Kurstyp (1 = Klassrum, 2 = Distans): ");
        if (int.TryParse(ReadLine(), out var value) && value is 1 or 2)
            return value == 1 ? CourseType.Klassrum : CourseType.Distans;
        Console.WriteLine("Ogiltigt val, ange 1 eller 2.");
    }
}

static void SeedTestData(EducationRegistry registry)
{
    var anna = new Teacher("Anna", "Svensson", "070-1112233", "800101-1234", "Storgatan 1", "411 01", "Göteborg",
        new[] { "C#", "Databaser" });
    var erik = new Teacher("Erik", "Lindgren", "070-2223344", "790202-2345", "Kungsgatan 5", "411 19", "Göteborg",
        new[] { "Webbutveckling", "JavaScript" });
    var leader = new EducationLeader("Lisa", "Berg", "070-3334455", "750303-3456", "Avenyn 10", "411 36", "Göteborg",
        new DateTime(2015, 8, 15), new[] { "Projektledning" });
    var admin = new Administrator("Olof", "Nilsson", "070-4445566", "700404-4567", "Hamngatan 3", "411 14", "Göteborg",
        new DateTime(2010, 1, 4));

    registry.AddTeacher(anna);
    registry.AddTeacher(erik);
    registry.AddEducationLeader(leader);
    registry.AddAdministrator(admin);

    var students = new[]
    {
        new Student("Maja", "Karlsson", "073-1000001", "950505-1111", "Vasagatan 2", "411 24", "Göteborg"),
        new Student("Oskar", "Holm", "073-1000002", "960606-2222", "Linnégatan 7", "413 04", "Göteborg"),
        new Student("Sara", "Ek", "073-1000003", "970707-3333", "Allén 4", "412 55", "Göteborg"),
        new Student("Jonas", "Dahl", "073-1000004", "980808-4444", "Parkvägen 9", "412 60", "Göteborg"),
        new Student("Elin", "Wallin", "073-1000005", "990909-5555", "Skolgatan 6", "413 12", "Göteborg"),
        new Student("Karl", "Persson", "073-1000006", "000101-6666", "Backgatan 8", "414 70", "Göteborg")
    };
    foreach (var student in students)
        registry.AddStudent(student);

    // Datum räknas från idag så att varningen för indragna kurser kan visas.
    var today = DateTime.Today;
    var csharp = new Course("C101", "Grundkurs i C#", today.AddDays(14), today.AddDays(54), CourseType.Klassrum);
    var web = new Course("W201", "Webbutveckling", today.AddDays(60), today.AddDays(120), CourseType.Distans);
    var db = new Course("D301", "Databaser med SQL", today.AddDays(90), today.AddDays(130), CourseType.Klassrum);
    var pm = new Course("P401", "Projektledning", today.AddDays(20), today.AddDays(45), CourseType.Distans);

    csharp.AssignTeacher(anna);
    web.AssignTeacher(erik);
    db.AssignTeacher(anna);
    pm.AssignTeacher(leader);

    foreach (var course in new[] { csharp, web, db, pm })
        registry.AddCourse(course);

    // Fem studenter på C#-kursen, två på webbkursen.
    foreach (var student in students.Take(5))
        csharp.AddStudent(student);
    web.AddStudent(students[0]);
    web.AddStudent(students[5]);

    Console.WriteLine("Kurser som riskerar att ställas in:");
    registry.ListCoursesAtRisk(today);
}
