using KITT.Telegram.Messages.Streaming;

namespace KITT.Telegram.Functions.Helpers;

public static class MessageConverter
{
    public static string ToText(StreamingScheduledMessage message)
    {
        var messageText = $"""
            *Nuova live "{message.StreamingTitle}" pianificata!*
            Il {message.StreamingScheduledDate.ToShortDateString()} si va live dalle {message.StreamingStartingTime.ToShortTimeString()} alle {message.StreamingEndingTime.ToShortTimeString()}.
            Vi aspetto su [{message.StreamingHostingChannelUrl}]({message.StreamingHostingChannelUrl}) per scoprire insieme di cosa tratteremo!
            Trovate maggiori informazioni su [https://live.morialberto.it/d/{message.StreamingSlug}](https://live.morialberto.it/d/{message.StreamingSlug})
            """;

        return Escape(messageText);
    }

    public static string ToText(StreamingCanceledMessage message)
    {
        var messageText = $"""
            La live "{message.StreamingTitle}" prevista per il giorno {message.StreamingScheduledDate.ToShortDateString()} dalle {message.StreamingStartingTime.ToShortTimeString()} alle {message.StreamingEndingTime.ToShortTimeString()} è stata annullata!
            Mi scuso per il problema. La recupereremo il prima possibile!
            """;

        return Escape(messageText);
    }

    public static string ToText(StreamingScheduleChangedMessage message)
    {
        var messageText = $"""
            La live "{message.StreamingTitle}" è stata spostata al giorno {message.StreamingScheduleDate.ToShortDateString()} dalle {message.StreamingStartingTime.ToShortTimeString()} alle {message.StreamingEndingTime.ToShortTimeString()}.
            Vi aspetto numerosi!
            Trovate maggiori informazioni su [https://live.morialberto.it/d/{message.StreamingSlug}](https://live.morialberto.it/d/{message.StreamingSlug})
            """;

        return Escape(messageText);
    }

    public static string ToText(StreamingVideoUploadedMessage message)
    {
        var messageText = $"""
            La registrazione della live "{message.StreamingTitle}" è stata caricata su YouTube.
            Trovate il video all'indirizzo [{message.YouTubeUrl}]({message.YouTubeUrl}).
            Qualsiasi feedback possiate avere, non esitate a contattarmi!
            Trovate maggiori informazioni su [https://live.morialberto.it/d/{message.StreamingSlug}](https://live.morialberto.it/d/{message.StreamingSlug})
            """;

        return Escape(messageText);
    }

    public static string ToText(StreamingHostingChannelChangedMessage message)
    {
        var messageText = $"""
            La live "{message.StreamingTitle}" si svolgerà sul canale [{message.StreamingHostingChannelUrl}]({message.StreamingHostingChannelUrl}) il giorno {message.StreamingScheduleDate.ToShortDateString()} alle {message.StreamingStartingTime.ToShortTimeString()}.
            Vi aspettiamo numerosi!
            Trovate maggiori informazioni su [https://live.morialberto.it/d/{message.StreamingSlug}](https://live.morialberto.it/d/{message.StreamingSlug})
            """;

        return Escape(messageText);
    }

    #region Private methods
    private static string Escape(string text)
    {
        char[] cr = new[] { '>', '#', '+', '=', '|', '{', '}', '.', '!', '-', '\\' };
        List<Char> chars = new();
        foreach (char c in text)
        {
            if (cr.Contains(c)) chars.Add('\\');
            chars.Add(c);
        }
        return new string(chars.ToArray());
    }
    #endregion
}
