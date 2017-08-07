using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SugaEngine.Export
{
    public static class Notes
    {
        const string ID_Info = "INFO";
        const string ID_Notes = "NOTES";
        const string ID_Song = "SONG";
        const int BufferSize = 1024;

        public static void Serialize(Music Music, string SongFilePath, string Directory)
        {
            SongFilePath = SongFilePath.Replace(@"\", "/");
            Directory = Directory.Replace(@"\", "/");

            System.IO.Directory.CreateDirectory(Directory);

            string audioPath = "/" + Music.Title + Path.GetExtension(SongFilePath).ToLower();
            Music.Song = audioPath;

            using (FileStream Output = new FileStream(Directory + "/" + Music.Title + ".sgsong", FileMode.Create, FileAccess.Write))
            {
                BinaryWriter bw = new BinaryWriter(Output);

                bw.Write(ID_Info);
                bw.Write(Music.Title);
                bw.Write(Music.BPM);
                bw.Write(Music.Delay);

                bw.Write(ID_Notes);
                bw.Write(Music.Notes.Count);
                foreach (Note note in Music.Notes)
                {
                    bw.Write((byte)note.Mode);
                    bw.Write(note.StartingTime.TotalMilliseconds);
                    bw.Write(note.EndingTime.TotalMilliseconds);
                    bw.Write(note.Lane);
                }

                bw.Write(ID_Song);
                bw.Write(Music.Song);
            }

            if(SongFilePath != Directory + Music.Song)
                File.Copy(SongFilePath, Directory + Music.Song, true);
        }

        public static Music Deserialize(string SGSongFilePath)
        {
            Music music = new Music();

            using (FileStream fs = new FileStream(SGSongFilePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                bool info = false, notes = false, song = false;
                while (!(info && notes && song))
                {
                    string id = br.ReadString();
                    switch (id)
                    {
                        case ID_Info:
                            music.Title = br.ReadString();
                            music.BPM = br.ReadInt32();
                            music.Delay = br.ReadInt32();
                            info = true;
                            break;
                        case ID_Notes:
                            music.Notes = new List<Note>();
                            int count = br.ReadInt32();
                            for (int i = 0; count > i; i++)
                            {
                                music.Notes.Add(new Note()
                                {
                                    Mode = (NoteMode)br.ReadByte(),
                                    StartingTime = TimeSpan.FromMilliseconds(br.ReadDouble()),
                                    EndingTime = TimeSpan.FromMilliseconds(br.ReadDouble()),
                                    Lane = br.ReadInt32()
                                });
                            }
                            notes = true;
                            break;
                        case ID_Song:
                            music.Song = br.ReadString();
                            song = true;
                            break;
                    }
                }
            }

            return music;
        }
    }
}
