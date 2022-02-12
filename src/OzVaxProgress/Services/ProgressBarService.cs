using System;

namespace OzVaxProgress.Services
{
    public class ProgressBarService
    {
        private const string SyringeFormat = @"
▐   ▄▄▄▄▄▄▄▄▄▄▄
▐▓{0}═══
▐   ▀▀▀▀▀▀▀▀▀▀▀";

        private const char FullBlock = '█';
        private const char HalfBlock = '▓';
        private const string EmptySection = "▒░░▒░░▒░░▒░░";
        
        public string CreateProgressBar(double fractionalPercent)
        {
            
            var proportionFull = fractionalPercent * EmptySection.Length;
            
            var fullBlocks = Math.Truncate(proportionFull);
            var halfBlocks = Math.Round(proportionFull - fullBlocks, MidpointRounding.AwayFromZero);

            return $"{new string(FullBlock, (int) fullBlocks)}{new string(HalfBlock, (int) halfBlocks)}{EmptySection.Substring((int)(fullBlocks+halfBlocks))}";
        }

        public string CreateSyringeProgressBar(double fractionalPercent)
        {
            return string.Format(SyringeFormat, CreateProgressBar(fractionalPercent));
        }
    }
}