using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Gemini
{
    private const string ApiUrl = "https://generativelanguage.googleapis.com/v1beta/interactions";
    private const string Model = "gemini-3.5-flash";

    public string apiPromptInit;
    public string apiPromptRefetch;
    public GameData apiResponse;

    public IEnumerator GetGameData()
    {
        // Read the API key from the environment instead of keeping it in the source code.
        // string apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        string apiKey = "";
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Debug.LogError("GEMINI_API_KEY environment variable is not set.");
            yield break;
        }

        apiPromptInit = "You are the brain behind the computer for a game. When the player is presented with a 6 sided cube that has a keyboard button pertaining to a specific letter on each side, the player has 1 minute to create as many words with the letters as possible by rotating the cube and pressing the keys. Your job is to present a random group of 6 letters. There should be at least 15 English words to make from the letters. You will present these words in a JSON object split into 4 arrays: common, uncommon, rare, and super rare. Based on how often the word is used in English conversation and literature, assign it to an array. The player is allowed to repeat letters but the final word must be 6 letters long. Return only the JSON with this data. Return this data for 5 letter groups.";
        apiPromptRefetch = "You are the brain behind the computer for a game. When the player is presented with a 6 sided cube that has a keyboard button pertaining to a specific letter on each side, the player has 1 minute to create as many words with the letters as possible by rotating the cube and pressing the keys. Your job is to present a random group of 6 letters. There should be at least 15 English words to make from the letters. You will present these words in a JSON object split into 4 arrays: common, uncommon, rare, and super rare. Based on how often the word is used in English conversation and literature, assign it to an array. The player is allowed to repeat letters but the final word must be 6 letters long. Return only the JSON with this data.";
        apiResponse = new GameData();

        // Gemini expects JSON with a model name and the text to send.
        string requestJson = JsonUtility.ToJson(new GeminiRequest
        {
            model = Model,
            input = apiPromptInit
        });

        using (UnityWebRequest request = new UnityWebRequest(ApiUrl, "POST"))
        {
            // UploadHandler sends our JSON. DownloadHandler stores Gemini's reply.
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestJson));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-goog-api-key", apiKey);

            // Wait here while the web request runs; Unity's game loop keeps going.
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(
                    $"Gemini request failed ({request.responseCode}): {request.error}\n" +
                    request.downloadHandler.text);
                yield break;
            }

            // The API returns its answer in the "output_text" JSON field.
            GeminiResponse response =
                JsonUtility.FromJson<GeminiResponse>(request.downloadHandler.text);

            if (response == null || string.IsNullOrEmpty(response.steps[1].content[0].text))
            {
                Debug.Log(response.steps[1].content[0].text);
                Debug.LogError($"Gemini did not return answer text: {request.downloadHandler.text}");
                yield break;
            }

            apiResponse.text = response.steps[1].content[0].text;
            Debug.Log(apiResponse.text);
        }
    }

    [Serializable]
    private class GeminiRequest
    {
        public string model;
        public string input;
    }

    [Serializable]
    private class GeminiResponse
    {
        public List<Step> steps;
    }

    [Serializable]
    private class Step
    {
        public List<Content> content;
    }

    [Serializable]
    private class Content
    {
        public string text;
        public string type;
    }
}

[Serializable]
public class GameData
{
    public string text;
    // public List<string> common;
    // public List<string> uncommon;
    // public List<string> rare;
    // public List<string> superRare;
}

