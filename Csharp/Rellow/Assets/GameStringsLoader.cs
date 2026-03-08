using FbonizziMonoGame.StringsLocalization.Abstractions;
using System.Globalization;

namespace Rellow.Assets
{
    public class GameStringsLoader
    {
        public const string YellowColorKey = "ColorsYellowKey";
        public const string RedColorKey = "ColorsRedKey";
        public const string GreenColorKey = "ColorsGreen";
        public const string BlueColorKey = "ColorsBlue";
        public const string OrangeColorKey = "ColorsOrange";
        public const string GrayColorKey = "ColorsGray";
        public const string WhiteColorKey = "ColorsWhite";
        public const string VioletColorKey = "ColorsViolet";
        public const string LightBlueColorKey = "ColorsLightBlue";

        public const string SecondaryLanguageYellowColorKey = "SecondaryLanguageColorsYellowKey";
        public const string SecondaryLanguageRedColorKey = "SecondaryLanguageColorsRedKey";
        public const string SecondaryLanguageGreenColorKey = "SecondaryLanguageColorsGreen";
        public const string SecondaryLanguageBlueColorKey = "SecondaryLanguageColorsBlue";
        public const string SecondaryLanguageOrangeColorKey = "SecondaryLanguageColorsOrange";
        public const string SecondaryLanguageGrayColorKey = "SecondaryLanguageColorsGray";
        public const string SecondaryLanguageWhiteColorKey = "SecondaryLanguageColorsWhite";
        public const string SecondaryLanguageVioletColorKey = "SecondaryLanguageColorsViolet";
        public const string SecondaryLanguageLightBlueColorKey = "SecondaryLanguageColorsLightBlue";

        public const string PlayStringKey = "Play";
        public const string AboutStringKey = "About";
        public const string PlayAgainStringKey = "PlayAgain";
        public const string ExitStringKey = "Exit";
        public const string ScoreStringKey = "Score";
        public const string GlobalRankStringKey = "GlobalRank";
        public const string GlobalRankLoadingStringKey = "GlobalRankLoadingStringKey";
        public const string GlobalRankCurrentlyUnavailableKey = "GlobalRankCurrentlyUnavailableKey";

        public GameStringsLoader(ILocalizedStringsRepository localizedStringsRepository, CultureInfo cultureInfo)
        {
            if (cultureInfo.TwoLetterISOLanguageName == "it")
            {
                localizedStringsRepository.AddString(YellowColorKey, "Giallo");
                localizedStringsRepository.AddString(RedColorKey, "Rosso");
                localizedStringsRepository.AddString(GreenColorKey, "Verde");
                localizedStringsRepository.AddString(BlueColorKey, "Blu");
                localizedStringsRepository.AddString(OrangeColorKey, "Arancione");
                localizedStringsRepository.AddString(GrayColorKey, "Grigio");
                localizedStringsRepository.AddString(WhiteColorKey, "Bianco");
                localizedStringsRepository.AddString(VioletColorKey, "Viola");
                localizedStringsRepository.AddString(LightBlueColorKey, "Azzurro");

                localizedStringsRepository.AddString(SecondaryLanguageYellowColorKey, "Yellow");
                localizedStringsRepository.AddString(SecondaryLanguageRedColorKey, "Red");
                localizedStringsRepository.AddString(SecondaryLanguageGreenColorKey, "Green");
                localizedStringsRepository.AddString(SecondaryLanguageBlueColorKey, "Blue");
                localizedStringsRepository.AddString(SecondaryLanguageOrangeColorKey, "Orange");
                localizedStringsRepository.AddString(SecondaryLanguageGrayColorKey, "Gray");
                localizedStringsRepository.AddString(SecondaryLanguageWhiteColorKey, "White");
                localizedStringsRepository.AddString(SecondaryLanguageVioletColorKey, "Violet");
                localizedStringsRepository.AddString(SecondaryLanguageLightBlueColorKey, "Light blue");

                localizedStringsRepository.AddString(PlayStringKey, "Gioca");
                localizedStringsRepository.AddString(AboutStringKey, "About");
                localizedStringsRepository.AddString(PlayAgainStringKey, "Gioca ancora");
                localizedStringsRepository.AddString(ExitStringKey, "Esci");
                localizedStringsRepository.AddString(ScoreStringKey, "Punti");
                localizedStringsRepository.AddString(GlobalRankStringKey, "Pos. globale");
                localizedStringsRepository.AddString(GlobalRankLoadingStringKey, "Caricamento classifica...");
                localizedStringsRepository.AddString(GlobalRankCurrentlyUnavailableKey, "Classifica non disponibile");
            }
            else
            {
                localizedStringsRepository.AddString(YellowColorKey, "Yellow");
                localizedStringsRepository.AddString(RedColorKey, "Red");
                localizedStringsRepository.AddString(GreenColorKey, "Green");
                localizedStringsRepository.AddString(BlueColorKey, "Blue");
                localizedStringsRepository.AddString(OrangeColorKey, "Orange");
                localizedStringsRepository.AddString(GrayColorKey, "Gray");
                localizedStringsRepository.AddString(WhiteColorKey, "White");
                localizedStringsRepository.AddString(VioletColorKey, "Violet");
                localizedStringsRepository.AddString(LightBlueColorKey, "Light blue");

                localizedStringsRepository.AddString(SecondaryLanguageYellowColorKey, "Yellow");
                localizedStringsRepository.AddString(SecondaryLanguageRedColorKey, "Red");
                localizedStringsRepository.AddString(SecondaryLanguageGreenColorKey, "Green");
                localizedStringsRepository.AddString(SecondaryLanguageBlueColorKey, "Blue");
                localizedStringsRepository.AddString(SecondaryLanguageOrangeColorKey, "Orange");
                localizedStringsRepository.AddString(SecondaryLanguageGrayColorKey, "Gray");
                localizedStringsRepository.AddString(SecondaryLanguageWhiteColorKey, "White");
                localizedStringsRepository.AddString(SecondaryLanguageVioletColorKey, "Violet");
                localizedStringsRepository.AddString(SecondaryLanguageLightBlueColorKey, "Light blue");

                localizedStringsRepository.AddString(PlayStringKey, "Play");
                localizedStringsRepository.AddString(AboutStringKey, "About");
                localizedStringsRepository.AddString(PlayAgainStringKey, "Play again");
                localizedStringsRepository.AddString(ExitStringKey, "Exit");
                localizedStringsRepository.AddString(ScoreStringKey, "Score");
                localizedStringsRepository.AddString(GlobalRankStringKey, "Global rank");
                localizedStringsRepository.AddString(GlobalRankLoadingStringKey, "Loading leaderboard...");
                localizedStringsRepository.AddString(GlobalRankCurrentlyUnavailableKey, "Leaderboard not available");
            }
        }
    }
}
