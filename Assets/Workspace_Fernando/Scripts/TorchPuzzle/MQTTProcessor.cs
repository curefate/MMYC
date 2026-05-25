using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using System.Collections.Generic;
using System;

public class MQTTProcessor : M2MqttUnityClient
{
    public int Hall_0 { get; private set; }
    public int Hall_1 { get; private set; }
    public int Hall_2 { get; private set; }
    public int Hall_3 { get; private set; }
    public int Hall_4 { get; private set; }
    public int Led_0 { get; private set; }
    public int Led_1 { get; private set; }
    public int Led_2 { get; private set; }
    public int Riddle { get; private set; }

    public static MQTTProcessor Instance { get; private set; } = null;
    public string StoryLanguage{ get; private set;} =  "en";

    private Dictionary<string, Action<string>> topicToProcessor;
    private readonly string mqttTopicPrefix = "MMYC/";


    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        topicToProcessor = new Dictionary<string, Action<string>>
        {
          { mqttTopicPrefix + "story_language", value =>
    {
        StoryLanguage = value.Trim().ToLower();
        Debug.Log("MQTT StoryLanguage changed to: " + StoryLanguage);
    }
},
            { mqttTopicPrefix + "hall_0", value => Hall_0 = int.TryParse(value, out var hall0) ? hall0 : Hall_0 },
            { mqttTopicPrefix + "hall_1", value => Hall_1 = int.TryParse(value, out var hall1) ? hall1 : Hall_1 },
            { mqttTopicPrefix + "hall_2", value => Hall_2 = int.TryParse(value, out var hall2) ? hall2 : Hall_2 },
            { mqttTopicPrefix + "hall_3", value => Hall_3 = int.TryParse(value, out var hall3) ? hall3 : Hall_3 },
            { mqttTopicPrefix + "hall_4", value => Hall_4 = int.TryParse(value, out var hall4) ? hall4 : Hall_4 },
            { mqttTopicPrefix + "led_0", value => Led_0 = int.TryParse(value, out var led0) ? led0 : Led_0 },
            { mqttTopicPrefix + "led_1", value => Led_1 = int.TryParse(value, out var led1) ? led1 : Led_1 },
            { mqttTopicPrefix + "led_2", value => Led_2 = int.TryParse(value, out var led2) ? led2 : Led_2 },
            { mqttTopicPrefix + "riddle", value => Riddle = int.TryParse(value, out var riddle) ? riddle : Riddle },
        };
    }

    protected override void DecodeMessage(string topic, byte[] message)
    {
        base.DecodeMessage(topic, message);

        var msg = System.Text.Encoding.UTF8.GetString(message);
        if (topicToProcessor.TryGetValue(topic, out var processor))
        {
            processor(msg);
        }
        else
        {
            Debug.LogWarning($"Received MQTT message on unrecognized topic {topic}: {msg}");
        }
    }

    protected override void SubscribeTopics()
    {
        foreach (var topic in topicToProcessor.Keys)
        {
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            Debug.Log($"Subscribed to topic {topic}");
        }
    }

    public void PublishMessage(string topic, string message)
    {
        if (client != null && client.IsConnected)
        {
            client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log($"Published message to topic {topic}: {message}");
        }
        else
        {
            Debug.LogWarning("Cannot publish MQTT message: client is not connected.");
        }
    }

    //Fer´s helpers
    public void TurnOnRedLED()
    {
        Debug.Log("TurnOnRedLED");
        PublishMessage("MMYC/led_0", "1");
    }

    public void TurnOnGreenLED()
    {
        Debug.Log("TurnOnGreenLED");
        PublishMessage("MMYC/led_1", "1");
    }

    public void TurnOnBlueLED()
    {
        Debug.Log("TurnOnBlueLED");
        PublishMessage("MMYC/led_2", "1");
    }

    public void TurnOffAllLEDs()
    {
        Debug.Log("TurnOffAllLEDs");
        PublishMessage("MMYC/led_0", "0");
        PublishMessage("MMYC/led_1", "0");
        PublishMessage("MMYC/led_2", "0");
    }
    // Jaz helper 
    public void TurnOnEN(){
        Debug.Log("TurnonEN");
        PublishMessage("MMYC/story_language", "en");
    }
    public void TurnOnSV()
    {
        Debug.Log("TurnOnSV");
        PublishMessage("MMYC/story_language", "sv");
    }
    

}
