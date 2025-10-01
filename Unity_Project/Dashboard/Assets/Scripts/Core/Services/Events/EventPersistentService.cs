using System.Collections.Generic;
using System.IO;
using Core.Data;
using Core.Interfaces.Services;
using UnityEngine;

namespace Core.Services.Events
{
    public class EventPersistenceService : IEventPersistenceService
    {
        private readonly string _filePath;
        private List<GameEventDto> _cachedEvents;

        public EventPersistenceService()
        {
            string projectRoot = Application.dataPath;  
            projectRoot = Directory.GetParent(projectRoot)!.FullName; 
            projectRoot = Directory.GetParent(projectRoot)!.FullName; 
            projectRoot = Directory.GetParent(projectRoot)!.FullName; 

            string etlDataDirectory = Path.Combine(projectRoot, "etl", "pythonProject", "data");
            _filePath = Path.Combine(etlDataDirectory, "events.json");

            _cachedEvents = new List<GameEventDto>();

            Debug.Log($"Events will be saved to: {_filePath}");

            if (!Directory.Exists(etlDataDirectory))
            {
                Directory.CreateDirectory(etlDataDirectory);
                Debug.Log($"Created ETL data directory at: {etlDataDirectory}");
            }

            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        _cachedEvents = JsonUtility.FromJson<EventListWrapper>(json)?.events ?? new List<GameEventDto>();
                        Debug.Log($"Loaded {_cachedEvents.Count} existing events from {_filePath}");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to load existing events: {ex.Message}");
                    _cachedEvents = new List<GameEventDto>();
                }
            }
            else
            {
                Debug.Log($"No existing events file found at {_filePath}. Starting with empty cache.");
            }
        }

        public void SaveEvent(GameEventDto gameEvent)
        {
            try
            {
                _cachedEvents.Add(gameEvent);

                var wrapper = new EventListWrapper { events = _cachedEvents };
                string json = JsonUtility.ToJson(wrapper, true);

                Debug.Log($"Attempting to write to: {_filePath}");
                Debug.Log($"Directory exists: {Directory.Exists(Path.GetDirectoryName(_filePath))}");

                File.WriteAllText(_filePath, json);
                
                if (File.Exists(_filePath))
                {
                    FileInfo fileInfo = new FileInfo(_filePath);
                    Debug.Log($"✓ Event saved successfully to: {_filePath}");
                    Debug.Log($"✓ File size: {fileInfo.Length} bytes");
                    Debug.Log($"✓ Total events: {_cachedEvents.Count}");
                }
                else
                {
                    Debug.LogError($"✗ File was not created at: {_filePath}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to save event: {ex.Message}");
                Debug.LogError($"Stack trace: {ex.StackTrace}");

                if (_cachedEvents.Count > 0)
                {
                    _cachedEvents.RemoveAt(_cachedEvents.Count - 1);
                }
            }
        }

        public string GetFilePath()
        {
            return _filePath;
        }

        public void TestFileWrite()
        {
            try
            {
                string testPath = Path.Combine(Path.GetDirectoryName(_filePath), "test.txt");
                string testContent = $"Test write at {System.DateTime.Now}";
                
                File.WriteAllText(testPath, testContent);
                Debug.Log($"✓ Test write successful to: {testPath}");
                
                string readBack = File.ReadAllText(testPath);
                Debug.Log($"✓ Test read successful: {readBack}");
                
                // Clean up test file
                File.Delete(testPath);
                Debug.Log("✓ Test file cleaned up");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"✗ Test failed: {ex.Message}");
            }
        }

        [System.Serializable]
        private class EventListWrapper
        {
            public List<GameEventDto> events;
        }
    }
}