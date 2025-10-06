namespace WonderSongs.Logic;

class WonderSongsPlayable
{
    MediaPlayer MediaPlayer { get; } = new();
    Dictionary<Song, int> PlayCounts { get; } = new(); // song → #times played
    List<Song> Songs { get; } = new();
    Song? PlayingSong { get; set; }

    public WonderSongsPlayable(Song[] initialList)
    {
        MediaPlayer.IsLoopingEnabled = false;
        MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

        Songs.AddRange(initialList);
        foreach (var s in initialList)
            PlayCounts[s] = 0;

        FillNextCandidate();
        MediaPlayer.PlaybackSession.PositionChanged += PlaybackSession_PositionChanged;

    }
    bool hasCrossed;

    private async void PlaybackSession_PositionChanged(MediaPlaybackSession sender, object args)
    {
        var position = sender.Position;
        if (!hasCrossed && sender.NaturalDuration - position <= TimeSpan.FromSeconds(30))
        {
            hasCrossed = true;
            var nextSong = await (NextSongsSelectionDialogTrigger?.Invoke() ?? Task.FromResult<Song?>(default)!);
            if (nextSong is not null)
            {
                ScheduleNextSong(nextSong);
            }
        }
    }


    double GetWeight(Song s) => 1.0 / (1 + PlayCounts[s]);

    void FillNextCandidate()
    {
        int count = Math.Min(3, Songs.Count);
        var available = new List<Song>(Songs);
        var candidates = new List<Song>(count);

        for (int i = 0; i < count; i++)
        {
            double totalWeight = available.Sum(s => GetWeight(s));
            double r = Random.Shared.NextDouble() * totalWeight;

            double cumulative = 0;
            Song? chosen = null;
            foreach (var s in available)
            {
                cumulative += GetWeight(s);
                if (r <= cumulative)
                {
                    chosen = s;
                    break;
                }
            }

            if (chosen != null)
            {
                candidates.Add(chosen);
                available.Remove(chosen); // no duplicates in this round
            }
        }

        NextCandidates = [.. candidates];
    }

    private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
    {
        PlayingSong = null;
        Play();
    }


    public Song[] NextCandidates { get; private set; } = [];
    public Song? NextSong { get; private set; }
    Action<Song>? NewSongPlaying;
    Func<Task<Song>>? NextSongsSelectionDialogTrigger;
    public void ScheduleNextSong(Song song)
    {
        NextSong = song;
        PlayCounts[song]++;        // increment only when actually scheduled
        FillNextCandidate();       // refresh candidates for the next round
        if (PlayingSong is null)
        {
            Play();
        }
    }
    public async void Play()
    {
        if (PlayingSong is null)
        {
            PlayingSong = NextSong;
            NextSong = null;
            if (PlayingSong is not null)
            {
                NewSongPlaying?.Invoke(PlayingSong);
                var src = await PlayingSong.GetSourceAsync();
                MediaPlayer.Source = src;
                hasCrossed = false;
            }
        }
        MediaPlayer.Play();
    }
    public void Pause() => MediaPlayer.Pause();
    public void LifeCycle(Func<Task<Song>> NextSongsSelectionDialogTrigger, Action<Song> NewSongPlaying)
    {
        this.NextSongsSelectionDialogTrigger = NextSongsSelectionDialogTrigger;
        this.NewSongPlaying = NewSongPlaying;

        InternalLifeCycle();

    }
    async void InternalLifeCycle()
    {
        // pick
        var song = await NextSongsSelectionDialogTrigger!();
        // play
        ScheduleNextSong(song);
        Play(); // triggers next song dialog
    }
}
