using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DatabaseWorkloadQueryGenerator.Application.Extensions;


/// <summary>
/// Extension methods for string operations
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Generates a random string with specific character type requirements
    /// </summary>
    /// <param name="_">The current string (ignored)</param>
    /// <param name="length">The length of the random string (defaults to 12)</param>
    /// <param name="hasAlphanumeric">Whether to include alphanumeric characters (defaults to true)</param>
    /// <returns>A randomly generated string with 1 alphanumeric, 8 characters, and 3 numbers in random order</returns>
    public static string GenerateRandom(this string _, int length = 12, bool hasAlphanumeric = true)
    {
        const string numbers = "0123456789";
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        const string alphanumericCharacters = "@#$%^&*";

        char[] result = new char[length];
        Random random = new();

        // Create a list of remaining positions
        List<int> availablePositions = Enumerable.Range(0, length).ToList();

        // Helper function to get a random position and remove it from available positions
        int GetRandomPosition()
        {
            int index = random.Next(availablePositions.Count);
            int position = availablePositions[index];
            availablePositions.RemoveAt(index);
            return position;
        }

        // Create a list of character types to place with their counts
        var characterTypes = new List<(string chars, int count)>()
            {
                (alphanumericCharacters, 1),
                (characters, 8),
                (numbers, 3)
            };

        // Randomize the order of character types
        characterTypes = characterTypes.OrderBy(_ => random.Next()).ToList();

        // Place characters in random order
        foreach (var (chars, count) in characterTypes)
        {
            for (int i = 0; i < count; i++)
            {
                int position = GetRandomPosition();
                result[position] = chars[random.Next(chars.Length)];
            }
        }

        return new string(result);
    }

    /// <summary>
    /// Converts a normal string to an ID-compatible string by removing special characters,
    /// replacing spaces with hyphens, and ensuring it's URL-safe
    /// </summary>
    /// <param name="input">The string to convert</param>
    /// <param name="maxLength">Maximum length of the resulting ID (defaults to 50)</param>
    /// <param name="separator">Character to use as word separator (defaults to '-')</param>
    /// <returns>An ID-compatible string</returns>
    public static string ToIdCompatible(this string input, int maxLength = 50, char separator = '-')
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Convert to lowercase and normalize
        string normalized = input.ToLowerInvariant().Trim();

        // Replace spaces and special characters with separator
        normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", string.Empty);
        normalized = Regex.Replace(normalized, @"\s+", separator.ToString());

        // Remove duplicate separators
        normalized = Regex.Replace(normalized, $"{separator}+", separator.ToString());

        // Trim separators from start and end
        normalized = normalized.Trim(separator);

        // Truncate to max length if needed
        if (normalized.Length > maxLength)
        {
            normalized = normalized.Substring(0, maxLength).TrimEnd(separator);
        }

        return normalized;
    }
}