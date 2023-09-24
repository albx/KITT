using KITT.Telegram.Functions.Helpers;
using KITT.Telegram.Messages.Streaming;
using System.Globalization;

namespace KITT.Telegram.Functions.Test.Helpers;

public class MessageConverterTest
{
    public MessageConverterTest()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    [Fact]
    public void ToText_Should_Convert_StreamingScheduledMessage_As_Expected()
    {
        var expectedMessageText = $"""
            *Nuova live "Live di test" pianificata\!*
            Il 03/10/2023 si va live dalle 21:00 alle 22:30\.
            Vi aspetto su [https://www\.twitch\.tv/albx87](https://www\.twitch\.tv/albx87) per scoprire insieme di cosa tratteremo\!
            Trovate maggiori informazioni su [https://live\.morialberto\.it/d/live\-di\-test](https://live\.morialberto\.it/d/live\-di\-test)
            """;

        var message = new StreamingScheduledMessage(
            StreamingId: Guid.NewGuid(),
            StreamingTitle: "Live di test",
            StreamingSlug: "live-di-test",
            StreamingScheduleDate: new DateOnly(2023, 10, 3),
            StreamingStartingTime: new TimeOnly(21, 0),
            StreamingEndingTime: new TimeOnly(22, 30),
            StreamingHostingChannelUrl: "https://www.twitch.tv/albx87");

        var messageText = MessageConverter.ToText(message);
        Assert.Equal(expectedMessageText, messageText);
    }

    [Fact]
    public void ToText_Should_Convert_StreamingCanceledMessage_As_Expected()
    {
        var expectedMessageText = $"""
            La live "Live di test" prevista per il giorno 03/10/2023 dalle 21:00 alle 22:30 è stata annullata\!
            Mi scuso per il problema\. La recupereremo il prima possibile\!
            """;

        var message = new StreamingCanceledMessage(
            StreamingId: Guid.NewGuid(),
            StreamingTitle: "Live di test",
            StreamingScheduleDate: new DateOnly(2023, 10, 3),
            StreamingStartingTime: new TimeOnly(21, 0),
            StreamingEndingTime: new TimeOnly(22, 30));

        var messageText = MessageConverter.ToText(message);
        Assert.Equal(expectedMessageText, messageText);
    }

    [Fact]
    public void ToText_Should_Convert_StreamingScheduleChangedMessage_As_Expected()
    {
        var expectedMessageText = $"""
            La live "Live di test" è stata spostata al giorno 03/10/2023 dalle 21:30 alle 23:00\.
            Vi aspetto numerosi\!
            Trovate maggiori informazioni su [https://live\.morialberto\.it/d/live\-di\-test](https://live\.morialberto\.it/d/live\-di\-test)
            """;

        var message = new StreamingScheduleChangedMessage(
            StreamingId: Guid.NewGuid(),
            StreamingTitle: "Live di test",
            StreamingSlug: "live-di-test",
            StreamingScheduleDate: new DateOnly(2023, 10, 3),
            StreamingStartingTime: new TimeOnly(21, 30),
            StreamingEndingTime: new TimeOnly(23, 00));

        var messageText = MessageConverter.ToText(message);
        Assert.Equal(expectedMessageText, messageText);
    }

    [Fact]
    public void ToText_Should_Convert_StreamingVideoUploadedMessage_As_Expected()
    {
        var expectedMessageText = $"""
            La registrazione della live "Live di test" è stata caricata su YouTube\.
            Trovate il video all'indirizzo [https://www\.youtube\.com/@albx87](https://www\.youtube\.com/@albx87)\.
            Qualsiasi feedback possiate avere, non esitate a contattarmi\!
            Trovate maggiori informazioni su [https://live\.morialberto\.it/d/live\-di\-test](https://live\.morialberto\.it/d/live\-di\-test)
            """;

        var message = new StreamingVideoUploadedMessage(
            StreamingId: Guid.NewGuid(),
            StreamingTitle: "Live di test",
            StreamingSlug: "live-di-test",
            YouTubeUrl: "https://www.youtube.com/@albx87");

        var messageText = MessageConverter.ToText(message);
        Assert.Equal(expectedMessageText, messageText);
    }

    [Fact]
    public void ToText_Should_Convert_StreamingHostingChannelChangedMessage_As_Expected()
    {
        var expectedMessageText = $"""
            La live "Live di test" si svolgerà sul canale [https://www\.twitch\.tv/albx87](https://www\.twitch\.tv/albx87) il giorno 03/10/2023 alle 21:00\.
            Vi aspettiamo numerosi\!
            Trovate maggiori informazioni su [https://live\.morialberto\.it/d/live\-di\-test](https://live\.morialberto\.it/d/live\-di\-test)
            """;

        var message = new StreamingHostingChannelChangedMessage(
            StreamingId: Guid.NewGuid(),
            StreamingTitle: "Live di test",
            StreamingSlug: "live-di-test",
            StreamingHostingChannelUrl: "https://www.twitch.tv/albx87",
            StreamingScheduleDate: new DateOnly(2023, 10, 3),
            StreamingStartingTime: new TimeOnly(21, 0));

        var messageText = MessageConverter.ToText(message);
        Assert.Equal(expectedMessageText, messageText);
    }
}
