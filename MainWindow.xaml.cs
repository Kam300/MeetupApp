using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace MeetupApp
{
    public partial class MainWindow : Window
    {
        private List<ProgramItem> programItems = new List<ProgramItem>();
        private string imagePath;

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 24; i++)
            {
                StartHourComboBox.Items.Add(i);
                EndHourComboBox.Items.Add(i);
            }
            for (int i = 0; i < 60; i++)
            {
                StartMinuteComboBox.Items.Add(i);
                EndMinuteComboBox.Items.Add(i);
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите отменить?", "Подтвердите", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TitleTextBox.Clear();
                DatePicker.SelectedDate = null;
                PlaceTextBox.Clear();
                DescriptionTextBox.Clear();
                ProgramListBox.Items.Clear();
                imagePath = null;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string message = $"Название: {TitleTextBox.Text}\nДата: {DatePicker.SelectedDate}\nМесто: {PlaceTextBox.Text}\n" +
                             $"Описание: {DescriptionTextBox.Text}\nИзображение: {imagePath}\nПрограмма: {string.Join(", ", programItems)}";

            MessageBox.Show(message, "Сохраненные данные");
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName;
                // Устанавливаем источник для изображения
                UploadedImage.Source = new BitmapImage(new Uri(imagePath));
                UploadedImage.Visibility = Visibility.Visible; // Показываем изображение
            }
        }


        private void AddProgramItemButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramItem newItem = new ProgramItem
            {
                Title = $"Пункт {programItems.Count + 1}",
                StartsAt = programItems.Count > 0 ? programItems[^1].EndsAt : DateTime.Now,
                EndsAt = DateTime.Now.AddHours(1) // Пример времени окончания
            };

            programItems.Add(newItem);
            ProgramListBox.Items.Add(newItem);
        }



        private void DeleteProgramItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramListBox.SelectedItem is ProgramItem selectedItem)
            {
                programItems.Remove(selectedItem);
                ProgramListBox.Items.Remove(selectedItem);
                EditProgramItemTextBox.Clear();
                EditProgramItemTextBox.IsEnabled = false;
                SaveEditedItemButton.IsEnabled = false;
            }
        }

        private void SaveEditedItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramListBox.SelectedItem is ProgramItem selectedItem)
            {
                // Получаем часы и минуты из ComboBox
                int startHour = (int)StartHourComboBox.SelectedItem;
                int startMinute = (int)StartMinuteComboBox.SelectedItem;
                int endHour = (int)EndHourComboBox.SelectedItem;
                int endMinute = (int)EndMinuteComboBox.SelectedItem;

                // Проверка для корректности
                if (endHour < startHour || (endHour == startHour && endMinute <= startMinute))
                {
                    MessageBox.Show("Время окончания должно быть позже времени начала.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                selectedItem.Title = EditProgramItemTextBox.Text;

                // Устанавливаем новые времена
                selectedItem.StartsAt = selectedItem.StartsAt.Date.Add(new TimeSpan(startHour, startMinute, 0));
                selectedItem.EndsAt = selectedItem.EndsAt.Date.Add(new TimeSpan(endHour, endMinute, 0));

                // Обновляем элемент в списке
                ProgramListBox.Items[ProgramListBox.SelectedIndex] = selectedItem;

                // Дополнительно, если вы хотите, чтобы список визуально обновился
                ProgramListBox.Items.Refresh();
                ProgramListBox.SelectedItem = selectedItem; // Снова выбираем измененный элемент
            }
        }


        private void ProgramListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ProgramListBox.SelectedItem is ProgramItem selectedItem)
            {
                EditProgramItemTextBox.Text = selectedItem.Title;

                // Устанавливаем выбранное время в ComboBox
                StartHourComboBox.SelectedItem = selectedItem.StartsAt.Hour;
                StartMinuteComboBox.SelectedItem = selectedItem.StartsAt.Minute;

                EndHourComboBox.SelectedItem = selectedItem.EndsAt.Hour;
                EndMinuteComboBox.SelectedItem = selectedItem.EndsAt.Minute;

                EditProgramItemTextBox.IsEnabled = true;
                SaveEditedItemButton.IsEnabled = true;
            }
        }



    }

    public class ProgramItem
    {
        public string Title { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }

        public override string ToString()
        {
            return $"{Title} ({StartsAt:HH:mm} - {EndsAt:HH:mm})";
        }
    }
}
