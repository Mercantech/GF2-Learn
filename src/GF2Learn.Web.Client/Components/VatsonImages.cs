namespace GF2Learn.Web.Client.Components;

public static class VatsonImages
{
    public static string ForVariant(string variant) => "/images/Vatson/" + (variant switch
    {
        "ok" => "Vatson_Giver_tommelfinger_op.png",
        "warn" => "Vatson_Undersoeger_med_forstoerrelsesglas.png",
        _ => "Vatson_Faar_en_ide_med_lysende_paere.png"
    });
}
