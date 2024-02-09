using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Input;

namespace Swordsman_Saga.Engine.InputManagement
{
    public interface IEvent
    {
        Keys GetKey();
    }
    [JsonConverter(typeof(EventConverter))]
    public class KeyEvent : IEvent
    {
        public EventType mEventType;
        public Keys mKey;
        public string Type => "KeyEvent";

        public KeyEvent(EventType eventType, Keys key)
        {
            mEventType = eventType;
            mKey = key;
        }

        public Keys GetKey()
        {
            return mKey;
        }
        
        public void SetKey(Keys key)
        {
            mKey = key;
        }
        
        public EventType GetEventType()
        {
            return mEventType;
        }
    }

    [JsonConverter(typeof(EventConverter))]
    public class MouseEvent : IEvent
    {
        public EventType mEventType;
        public MouseButton mButton;
        public string Type => "MouseEvent";

        public MouseEvent(EventType eventType, MouseButton button)
        {
            mEventType = eventType;
            mButton = button;
        }

        public Keys GetKey()
        {
            return Keys.None;
        }

        public EventType GetEventType()
        {
            return mEventType;
        }
    }

    public class EventConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IEvent).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            JObject jsonObject = JObject.Load(reader);

            if (jsonObject.TryGetValue("Type", StringComparison.OrdinalIgnoreCase, out JToken typeToken))
            {
                switch (typeToken.Value<string>().ToLower())
                {
                    case "keyevent":
                        return new KeyEvent(
                            (EventType)Enum.Parse(typeof(EventType), jsonObject["mEventType"].ToString(), true),
                            (Keys)Enum.Parse(typeof(Keys), jsonObject["mKey"].ToString(), true)
                        );
                    case "mouseevent":
                        return new MouseEvent(
                            (EventType)Enum.Parse(typeof(EventType), jsonObject["mEventType"].ToString(), true),
                            (MouseButton)Enum.Parse(typeof(MouseButton), jsonObject["mButton"].ToString(), true)
                        );
                }
            }

            return null;
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            JObject jsonObject = new JObject();

            if (value is KeyEvent keyEvent)
            {
                jsonObject.Add("Type", keyEvent.Type);
                jsonObject.Add("mEventType", keyEvent.GetEventType().ToString());
                jsonObject.Add("mKey", keyEvent.GetKey().ToString());
            }
            else if (value is MouseEvent mouseEvent)
            {
                jsonObject.Add("Type", mouseEvent.Type);
                jsonObject.Add("mEventType", mouseEvent.GetEventType().ToString());
                jsonObject.Add("mButton", mouseEvent.mButton.ToString());
            }

            jsonObject.WriteTo(writer);
        }
    }
}

