﻿using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinder
{
    public class MessageModel
    {
        public int ID { get; set; }
        /// <summary>
        /// Connected with fkey to <see cref="RelationShipModel.ID"/>
        /// </summary>
        public int RelationShipID { get; set; }
        public string Text { get; set; }
        public int Sender { get; set; }
        public DateTime Timestamp { get; set; }
        public int HasBeenRead { get; set; }
    }
}
