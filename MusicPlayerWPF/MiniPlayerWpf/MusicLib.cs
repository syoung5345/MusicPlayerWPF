﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniPlayerWpf
{
    public class MusicLib
    {
        private DataSet musicDataSet;

        public EnumerableRowCollection<string> SongIds
        {
            get
            {
                var ids = from row in musicDataSet.Tables["song"].AsEnumerable()
                          orderby row["id"]
                          select row["id"].ToString();
                return ids;
            }
        }

        public MusicLib()
        {
            musicDataSet = new DataSet();
            musicDataSet.ReadXmlSchema("music.xsd");
            musicDataSet.ReadXml("music.xml");
        }

        public int AddSong(Song song)
        {
            // Add the selected file to the song table
            DataTable table = musicDataSet.Tables["song"];
            DataRow row = table.NewRow();

            row["title"] = song.Title;
            row["artist"] = song.Artist;
            row["album"] = song.Album;
            row["filename"] = song.Filename;
            row["length"] = song.Length;
            row["genre"] = song.Genre;
            table.Rows.Add(row);

            // Now we know the ID
            song.Id = Convert.ToInt32(row["id"]);

            return song.Id;
        }

        public Song GetSong(int songId)
        {
            Song song = null;
            DataTable table = musicDataSet.Tables["song"];

            // Only one row should be selected
            foreach (DataRow row in table.Select("id=" + songId))
            {
                song = new Song();
                song.Id = songId;
                song.Title = row["title"].ToString();
                song.Artist = row["artist"].ToString();
                song.Album = row["album"].ToString();
                song.Genre = row["genre"].ToString();
                song.Length = row["length"].ToString();
                song.Filename = row["filename"].ToString();
            }

            return song;
        }

        public bool UpdateSong(int songId, Song song)
        {
            bool result = false;
            DataTable table = musicDataSet.Tables["song"];

            if (songId > 0 && songId <= SongIds.Count())
            {
                // Only one row should be selected
                foreach (DataRow row in table.Select("id=" + songId))
                {
                    row["title"] = song.Title;
                    row["artist"] = song.Artist;
                    row["album"] = song.Album;
                    row["genre"] = song.Genre;
                    row["length"] = song.Length;
                    row["filename"] = song.Filename;
                }
                result = true;
            }

            return result;
        }

        public bool DeleteSong(int songId)
        {
            bool result = true;

            if (songId > 0 && songId <= SongIds.Count())
            {
                // Search the primary key for the selected song and delete it from 
                // the song table
                DataTable table = musicDataSet.Tables["song"];
                table.Rows.Remove(table.Rows.Find(songId));

                // Remove from playlist_song every occurance of songId.
                // Add rows to a separate list before deleting because we'll get an exception
                // if we try to delete more than one row while looping through table.Rows

                List<DataRow> rows = new List<DataRow>();
                table = musicDataSet.Tables["playlist_song"];
                foreach (DataRow row in table.Rows)
                    if (row["song_id"].ToString() == songId.ToString())
                        rows.Add(row);

                foreach (DataRow row in rows)
                    row.Delete();
            }

            return result;
        }

        public void Save(string filename)
        {
            musicDataSet.WriteXml(filename);
        }

        public void PrintAllTables()
        {
            foreach (DataTable table in musicDataSet.Tables)
            {
                Console.WriteLine("Table name = " + table.TableName);
                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine("Row:");
                    int i = 0;
                    foreach (Object item in row.ItemArray)
                    {
                        Console.WriteLine(" " + table.Columns[i].Caption + "=" + item);
                        i++;
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
