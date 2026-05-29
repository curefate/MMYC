using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using System.Collections.Generic;
using System;

public class MQTTProcessor : M2MqttUnityClient
{
    public int Hall_base { get; private set; }
    public int Hall_0 { get; private set; }
    public int Hall_1 { get; private set; }
    public int Hall_2 { get; private set; }
    public int Hall_3 { get; private set; }
    public int Hall_4 { get; private set; }
    public int Led_0 { get; private set; }
    public int Led_1 { get; private set; }
    public int Led_2 { get; private set; }
    public int Language { get; private set; }

    // 1: Activate pillar
    // 2: Light up red torch
    // 3: Light up green torch
    // 4: Light up blue torch
    // 5: Force scale equal
    // 6: CoffinAnim manually
    // 7: Answer correct
    // 8: Answer wrong
    // 9: Calibration hall base
    public int CheatCode { get; private set; }

    public static MQTTProcessor Instance { get; private set; } = null;

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
            { mqttTopicPrefix + "hall_base", value => Hall_base = int.TryParse(value, out var hallBase) ? hallBase : Hall_base },
            { mqttTopicPrefix + "hall_0", value => Hall_0 = int.TryParse(value, out var hall0) ? hall0 : Hall_0 },
            { mqttTopicPrefix + "hall_1", value => Hall_1 = int.TryParse(value, out var hall1) ? hall1 : Hall_1 },
            { mqttTopicPrefix + "hall_2", value => Hall_2 = int.TryParse(value, out var hall2) ? hall2 : Hall_2 },
            { mqttTopicPrefix + "hall_3", value => Hall_3 = int.TryParse(value, out var hall3) ? hall3 : Hall_3 },
            { mqttTopicPrefix + "hall_4", value => Hall_4 = int.TryParse(value, out var hall4) ? hall4 : Hall_4 },
            { mqttTopicPrefix + "led_0", value => Led_0 = int.TryParse(value, out var led0) ? led0 : Led_0 },
            { mqttTopicPrefix + "led_1", value => Led_1 = int.TryParse(value, out var led1) ? led1 : Led_1 },
            { mqttTopicPrefix + "led_2", value => Led_2 = int.TryParse(value, out var led2) ? led2 : Led_2 },
            { mqttTopicPrefix + "language", value => Language = int.TryParse(value, out var language) ? language : Language },
            { mqttTopicPrefix + "cheat_code", value => CheatCode = int.TryParse(value, out var cheatCode) ? cheatCode : CheatCode },
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
}
