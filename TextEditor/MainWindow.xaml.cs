using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    // абстрактный класс, который реализуют конкретные классы
    abstract class Command
    {
        public abstract void Execute(TextBox tb);
        public abstract void Undo(TextBox tb);
    }

    // конкретная команда
    class OpenFile : Command
    {
        FileOpener fileOpener; // экземпляр получателя команды
        public OpenFile(FileOpener fileCreator)
        {
            this.fileOpener = fileCreator;
        }
        public override void Execute(TextBox tb)
        {
            fileOpener.Open(tb);
        }

        public override void Undo(TextBox tb)
        {
            tb.Text = "";
        }
    }

    // конкретная команда
    class SaveFile : Command
    {
        FileSaver fileSaver;
        public SaveFile(FileSaver fileSaver)
        {
            this.fileSaver = fileSaver;
        }
        public override void Execute(TextBox tb) // выполнение
        {
            fileSaver.Save(tb);
        }

        public override void Undo(TextBox tb) // отмена
        {
            tb.Text = ""; // очистка текстового поля
            try
            {
                System.IO.File.Delete(fileSaver.FileName); // удаление файла
                MessageBox.Show(fileSaver.FileName + " был удален");
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось удалить " + fileSaver.FileName);
            }
        }
    }

    // получатель команды
    class FileOpener
    {
        public void Open(TextBox tb) // открытие
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Все файлы (*.*)|*.* ";
            openFileDialog.ShowDialog();
            string fileName = openFileDialog.FileName;
            string fileText = System.IO.File.ReadAllText(fileName);
            tb.Text = fileText;
            MessageBox.Show("Файл открыт");
        }
    }
    
    // получатель команды
    class FileSaver
    {
        public string FileName { get; set; } // имя файла, чтобы потом его можно было удалить при отмене
        public void Save(TextBox tb) // сохранение
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Все файлы (*.*)|*.* ";
            saveFileDialog.ShowDialog();
            FileName = saveFileDialog.FileName;

            System.IO.File.WriteAllText(FileName, tb.Text);
            MessageBox.Show("Файл сохранен");
        }
    }

    // Инициатор
    class File
    {
        Command command; // экземпляр команды
        public void SetCommand(Command c) // присвоение значения команде
        {
            command = c;
        }
        public void Run(TextBox tb) // запуск
        {
            command.Execute(tb);
        }
        public void Cancel(TextBox tb) // отмена
        {
            command.Undo(tb);
        }
    }


    public partial class MainWindow : Window
    {
        File file = new File(); // инициализируем файл, из которого будем потом вызывать методы
        public MainWindow()
        {
            InitializeComponent();            
        }

        private void Open(object sender, RoutedEventArgs e) // открытие файла
        {
            file.SetCommand(new OpenFile(new FileOpener())); // задаем команду
            file.Run(textBox); // запускаем команду
        }

        private void Save(object sender, RoutedEventArgs e) // сохранение в файл
        {
            file.SetCommand(new SaveFile(new FileSaver())); // задаем команду
            file.Run(textBox); // запускаем команду
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            file.Cancel(textBox); //отмена команды
        }
    }
}