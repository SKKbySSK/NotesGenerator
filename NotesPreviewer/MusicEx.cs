using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SugaEngine;

namespace NotesPreviewer
{
    class PlayableMusic : Music
    {
        public PlayableMusic(Music Music) : base(Music)
        {
            foreach(Note note in Music.Notes)
            {
                PlayableNotes.Add(new PlayableNote(note, false));
            }
        }

        public List<PlayableNote> PlayableNotes { get; } = new List<PlayableNote>();
    }

    class PlayableNote : Note
    {
        public PlayableNote(Note Note, bool Played) : base(Note)
        {
            HasPlayed = Played;
        }

        public bool HasPlayed { get; set; }
    }
}
