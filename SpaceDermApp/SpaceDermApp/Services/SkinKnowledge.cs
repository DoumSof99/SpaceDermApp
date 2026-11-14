namespace SpaceDermApp.Services;

public record Source(string Title, string Url);

public record ExampleCase(string Label, string Text);

public static class SkinKnowledge
{
    public const string SystemPrompt = """
        You are a medical decision-support assistant for the crew doctor on a long mission to Mars.
        The person writing to you IS the crew doctor, a qualified physician. They are not the patient.
        Earth is 3 to 22 minutes away one way, so the doctor needs ideas now.
        The doctor describes a skin problem seen on a crew member. You give ideas; the doctor makes the diagnosis.
        Always answer. Never refuse and never tell the doctor to see a doctor.

        Background facts about skin in space:
        - Skin rashes are one of the most common medical problems on the International Space Station.
        - In space, skin heals more slowly and becomes thinner.
        - Sweat does not drip in microgravity. It stays on the skin unless it is wiped off,
          so sweat, friction and moisture can irritate the skin (for example after exercise).

        Answer in English, in short bullet points, with exactly these four headings:
        Possible causes:
        Questions to ask the crew member:
        What to check or watch:
        Warning signs:

        List up to 3 possible causes, most likely first, and never say you are sure.
        Do not give medicine doses. Keep the whole answer under 220 words.
        Stop after the Warning signs section.
        """;

    public static string WrapCase(string doctorText) =>
        $"Case description from the crew doctor: {doctorText}\nGive your ideas using the four headings.";

    public static string WrapFollowUp(string doctorText) =>
        $"Follow-up question from the crew doctor about the same case: {doctorText}\nAnswer briefly in bullet points.";

    public const string RetryNote =
        "Reminder: you are talking to the crew doctor, a qualified physician, not a patient. " +
        "Do not refuse. Answer now.";

    public static readonly string[] FollowUps =
    [
        "What should I ask the crew member first?",
        "Which signs mean I should contact Earth?",
        "How can we prevent this for the rest of the mission?",
        "What should I check again tomorrow?",
    ];

    public static bool LooksLikeRefusal(string answer)
    {
        var start = answer.Length > 300 ? answer[..300] : answer;
        string[] signs = ["I cannot", "I can't", "I'm not able", "I am not able", "I'm unable",
                          "consult a qualified", "consult a medical", "seek medical"];
        return signs.Any(s => start.Contains(s, StringComparison.OrdinalIgnoreCase));
    }

    public static readonly ExampleCase[] Examples =
    [
        new("Rash after exercise",
            "Red, itchy patch on the lower back, about 4 cm, for 3 days. It started after daily exercise sessions. No fever."),
        new("Painful blisters",
            "Small painful blisters in a band on the right side of the chest, with burning pain for 2 days. The crew member feels tired."),
        new("Dry, cracked hands",
            "Dry, flaky and cracked skin on both hands, worse after using wet wipes for hygiene. Day 140 of the mission."),
        new("Red skin around a cut",
            "Red, swollen and warm skin around a small cut on the left shin, for 1 day. Mild pain when touched."),
    ];

    public static readonly Source[] Sources =
    [
        new("NASA NTRS: skin rashes on the ISS", "https://ntrs.nasa.gov/citations/20090001314"),
        new("Skin changes in spaceflight (review)", "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC9934743/"),
        new("Astronauts' skin heals more slowly", "https://phys.org/news/2015-07-astronauts-skin-missions.html"),
        new("Sweat does not drip in microgravity (NPR)", "https://www.kunc.org/2022-08-22/what-happens-to-sweat-in-outer-space-hint-theres-no-gravity-to-help-it-drip-away"),
        new("NASA NTRS: Mars communication delays", "https://ntrs.nasa.gov/citations/20220013418"),
        new("NASA NTRS: AI support for the crew doctor (EIMO)", "https://ntrs.nasa.gov/citations/20240011647"),
    ];
}
