using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace Tetris
{
    public static class AudioManager
    {
        // Основний медіаплеєр для фонової музики
        private static readonly MediaPlayer mediaPlayer = new MediaPlayer();

        // Плейліст і поточний трек
        private static List<string> playlist = new List<string>();
        private static int currentTrackIndex = 0;

        // Шляхи до папок з музикою та звуками
        private static readonly string musicFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Music");
        private static readonly string soundsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");

        // Прапорці ініціалізації
        private static bool isInitialized = false;
        private static bool mediaEndedSubscribed = false;

        // Метод для відтворення фонової музики
        public static void PlayBackgroundMusic(GameSettings settings)
        {
            // Перевірка налаштувань - якщо музика вимкнена, зупиняємо відтворення
            if (settings == null || !settings.MusicEnabled)
            {
                StopBackgroundMusic();
                return;
            }

            // Завантажуємо плейліст
            LoadPlaylist();

            // Якщо плейліст порожній - виходимо
            if (playlist.Count == 0)
                return;

            // Ініціалізація при першому запуску
            if (!isInitialized)
            {
                currentTrackIndex = 0;
                PlayTrack(currentTrackIndex, settings.Volume);

                // Підписка на подію завершення треку (для автоматичного переходу до наступного)
                if (!mediaEndedSubscribed)
                {
                    mediaPlayer.MediaEnded += (s, e) => NextTrack(settings);
                    mediaEndedSubscribed = true;
                }

                isInitialized = true;
            }
            else
            {
                // Оновлення гучності та продовження відтворення
                mediaPlayer.Volume = settings.Volume / 100.0;
                mediaPlayer.Play();
            }
        }

        // Завантаження плейлісту з папки Music
        private static void LoadPlaylist()
        {
            if (Directory.Exists(musicFolderPath))
            {
                // Отримуємо всі mp3-файли з папки
                playlist = Directory.GetFiles(musicFolderPath, "*.mp3").ToList();
                currentTrackIndex = 0;
                isInitialized = false;
                mediaEndedSubscribed = false;
            }
            else
            {
                // Якщо папки не існує - очищаємо плейліст
                playlist.Clear();
                currentTrackIndex = 0;
                isInitialized = false;
                mediaEndedSubscribed = false;
            }
        }

        // Відтворення конкретного треку за індексом
        private static void PlayTrack(int index, int volume)
        {
            // Перевірка коректності індексу
            if (playlist.Count == 0 || index < 0 || index >= playlist.Count)
                return;

            // Відкриття та відтворення треку
            mediaPlayer.Open(new Uri(playlist[index], UriKind.Absolute));
            mediaPlayer.Volume = volume / 100.0; // Нормалізація гучності (0-100 до 0.0-1.0)
            mediaPlayer.Play();
        }

        // Перехід до наступного треку
        public static void NextTrack(GameSettings settings)
        {
            if (playlist.Count == 0)
                return;

            // Круговий індекс (після останнього треку - перший)
            currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;
            PlayTrack(currentTrackIndex, settings.Volume);
        }

        // Оновлення гучності відповідно до налаштувань
        public static void UpdateVolume(GameSettings settings)
        {
            if (settings == null)
                return;

            if (settings.MusicEnabled)
            {
                mediaPlayer.Volume = settings.Volume / 100.0;

                // Якщо немає поточного треку - запускаємо фонову музику
                if (mediaPlayer.Source == null)
                {
                    PlayBackgroundMusic(settings);
                }
                else
                {
                    mediaPlayer.Play();
                }
            }
            else
            {
                StopBackgroundMusic();
            }
        }

        // Зупинка фонової музики
        public static void StopBackgroundMusic()
        {
            mediaPlayer.Pause();
        }

        // Відтворення звуку очищення лінії
        public static void PlayLineClearSound(GameSettings settings)
        {
            if (settings == null || !settings.SoundEnabled)
                return;

            // Шлях до звукового файлу
            string soundFile = Path.Combine(soundsFolderPath, "lineclear.mp3");

            if (!File.Exists(soundFile))
                return;

            // Створення нового медіаплеєра для звукового ефекту
            var player = new MediaPlayer
            {
                Volume = settings.Volume / 100.0
            };

            player.Open(new Uri(soundFile, UriKind.Absolute));

            // Автоматичне закриття плеєра після завершення відтворення
            player.MediaEnded += (s, e) =>
            {
                player.Close();
            };

            player.Play();
        }
    }
}