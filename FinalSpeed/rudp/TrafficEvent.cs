﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FinalSpeed.rudp
{
    public class TrafficEvent
    {
        long eventId;

        int traffic;

        public static int type_downloadTraffic = 10;

        public static int type_uploadTraffic = 11;

        int type = type_downloadTraffic;

        string userId;

        public TrafficEvent(long eventId, int traffic, int type)
            : this(null, eventId, traffic, type)
        {

        }

        public TrafficEvent(string userId, long eventId, int traffic, int type)
        {
            this.userId = userId;
            this.eventId = eventId;
            this.traffic = traffic;
            this.type = type;
        }

        public string getUserId()
        {
            return userId;
        }

        public void setUserId(string userId)
        {
            this.userId = userId;
        }

        public int getType()
        {
            return type;
        }

        public long getEventId()
        {
            return eventId;
        }

        public void setEventId(long eventId)
        {
            this.eventId = eventId;
        }

        public int getTraffic()
        {
            return traffic;
        }

        public void setTraffic(int traffic)
        {
            this.traffic = traffic;
        }

    }
}
