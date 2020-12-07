using System.Collections.Generic;

namespace F1_Data_Management
{
    public static class TrackTurns
    {
        static Dictionary<Track, TrackTurnInfo> _trackInfo = new Dictionary<Track, TrackTurnInfo>()
        {
            {
                Track.Yas_Marina_Circuit, //DONE
                new TrackTurnInfo(new Turn[] 
                {
                    new Turn(305, 530),   //1
                    new Turn(568, 750),   //2
                    new Turn(750, 1005),  //3
                    new Turn(1005, 1218), //4
                    new Turn(1218, 1348), //5
                    new Turn(1348, 1435), //6
                    new Turn(1435, 1607), //7
                    new Turn(2665, 2804), //8
                    new Turn(2804, 2915), //9
                    new Turn(2915, 3173), //10
                    new Turn(3715, 3829), //11
                    new Turn(3829, 3883), //12
                    new Turn(3883, 3983), //13
                    new Turn(3983, 4135), //14
                    new Turn(4274, 4425), //15
                    new Turn(4425, 4578), //16
                    new Turn(4578, 4666), //17
                    new Turn(4666, 4800), //18
                    new Turn(4800, 4965), //19
                    new Turn(5092, 5264), //20
                    new Turn(5264, 5470), //21
                })
            },
            {
                Track.Red_Bull_Ring, //DONE
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(315, 546),   //1
                    new Turn(999, 1232),  //2
                    new Turn(1284, 1476), //3
                    new Turn(2117, 2317), //4
                    new Turn(2317, 2586), //5
                    new Turn(2586, 2894), //6
                    new Turn(2894, 3155), //7
                    new Turn(3155, 3387), //8
                    new Turn(3681, 3922), //9
                    new Turn(3922, 4154), //10
                })
            },
            {
                Track.Baku_City_Circuit,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                    new Turn(0, 0), //18
                    new Turn(0, 0), //19
                    new Turn(0, 0), //20
                })
            },
            {
                Track.Autodromo_Jose_Carlos_Pace,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                })
            },
            {
                Track.Cicuit_de_Barcelona_Catalunya,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                })
            },
            {
                Track.Hanoi_Street_Circuit,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                    new Turn(0, 0), //18
                    new Turn(0, 0), //19
                    new Turn(0, 0), //20
                    new Turn(0, 0), //21
                    new Turn(0, 0), //22
                    new Turn(0, 0), //23
                })
            },
            {
                Track.Hockenheimring,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0), //1
                    new Turn(0, 0), //2
                    new Turn(0, 0), //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                })
            },
            {
                Track.Hungaroring,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                })
            },
            {
                Track.Albert_Park_Circuit,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                })
            },
            {
                Track.Autodromo_Hermanos_Rodriguez,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0), //1
                    new Turn(0, 0), //2
                    new Turn(0, 0), //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                })
            },
            {
                Track.Circuit_de_Monaco,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                    new Turn(0, 0), //18
                    new Turn(0, 0), //19
                })
            },
            {
                Track.Circuit_Gilles_Villeneuve,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                })
            },
            {
                Track.Autodromo_Nazionale_Monza,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                })
            },
            {
                Track.Circuit_Paul_Ricard,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                })
            },
            {
                Track.Bahrain_International_Circuit,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                })
            },
            {
                Track.Bahrain_International_Circuit_Short,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                })
            },
            {
                Track.Shanghai_International_Circuit,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                })
            },
            {
                Track.Silverstone_Circuit,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                    new Turn(0, 0), //18
                })
            },
            {
                Track.Silverstone_Circuit_Short,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                })
            },
            {
                Track.Marina_Bay_Street_Circuit,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                    new Turn(0, 0), //18
                    new Turn(0, 0), //19
                    new Turn(0, 0), //20
                    new Turn(0, 0), //21
                    new Turn(0, 0), //22
                    new Turn(0, 0), //23
                })
            },
            {
                Track.Sochi_Autodrom,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                    new Turn(0, 0), //18
                })
            },
            {
                Track.Circuit_de_Spa_Francorchamps,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                    new Turn(0, 0), //18
                    new Turn(0, 0), //19
                })
            },
            {
                Track.Suzuka_Circuit,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                    new Turn(0, 0), //18
                })
            },
            {
                Track.Suzuka_Circuit_Short,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                })
            },
            {
                Track.Circuit_of_the_Americas,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                    new Turn(0, 0), //18
                    new Turn(0, 0), //19
                    new Turn(0, 0), //20
                })
            },
            {
                Track.Circuit_of_the_Americas_Short,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                    new Turn(0, 0), //15
                    new Turn(0, 0), //16
                    new Turn(0, 0), //17
                    new Turn(0, 0), //18
                })
            },
            {
                Track.Circuit_Zandvoort,
                new TrackTurnInfo(new Turn[]
                {
                    new Turn(0, 0),   //1
                    new Turn(0, 0),   //2
                    new Turn(0, 0),  //3
                    new Turn(0, 0), //4
                    new Turn(0, 0), //5
                    new Turn(0, 0), //6
                    new Turn(0, 0), //7
                    new Turn(0, 0), //8
                    new Turn(0, 0), //9
                    new Turn(0, 0), //10
                    new Turn(0, 0), //11
                    new Turn(0, 0), //12
                    new Turn(0, 0), //13
                    new Turn(0, 0), //14
                })
            },
        };

        /// <summary>
        /// Return turn number for specific track and distance along a lap of that track.
        /// </summary>
        /// <param name="track">What track are you racing at</param>
        /// <param name="lapDistance">How far along in meters are you around the track</param>
        /// <returns>Return turn number. Returns 0 if no turn.</returns>
        public static int GetTurn(Track track, float lapDistance)
        {
            if (_trackInfo.ContainsKey(track))
                return _trackInfo[track].GetTurn(lapDistance);

            throw new System.Exception("There is no turn support for track: " + track);
        }

        public struct TrackTurnInfo
        {
            /// <summary>
            /// All turns for specific track. Are in order.
            /// </summary>
            public Turn[] turns;

            /// <summary>
            /// Sets all turns for a specific track.
            /// </summary>
            /// <param name="track">What track.</param>
            /// <param name="turns">All turns for specific track lap. Must be in order.</param>
            public TrackTurnInfo(Turn[] turns)
            {
                this.turns = turns;
            }

            /// <summary>
            /// Returns turn number if specified lapDistance is inside a turn. 0 otherwise.
            /// </summary>
            /// <param name="lapDistance">Distance along lap in meters</param>
            /// <returns>Turn number. 0 if not in a turn.</returns>
            public int GetTurn(float lapDistance)
            {
                int min = 0;
                int max = turns.Length - 1;

                while (min <= max)
                {
                    int mid = (min + max) / 2;
                    float midStartPoint = turns[mid].startPoint;
                    if (turns[mid].IsTurn(lapDistance))
                        return mid + 1;
                    else if (lapDistance < midStartPoint)
                        max = mid - 1;
                    else
                        min = mid + 1;
                }

                //No valid turn was found
                return 0;
            }
        }

        public struct Turn
        {
            /// <summary>
            /// Start point along lap in meter for turn.
            /// </summary>
            public float startPoint;
            /// <summary>
            /// End point along lap in meter for turn.
            /// </summary>
            public float endPoint;

            /// <summary>
            /// Creates a new Turn for specific place on track.
            /// </summary>
            /// <param name="startPoint">Start point along lap in meter for turn.</param>
            /// <param name="endPoint">End point along lap in meter for turn.</param>
            public Turn(float startPoint, float endPoint)
            {
                this.startPoint = startPoint;
                this.endPoint = endPoint;
            }

            /// <summary>
            /// Returns if you are currently in corner.
            /// </summary>
            /// <param name="lapDistance">How far along the lap you are in meters</param>
            public bool IsTurn(float lapDistance)
            {
                return lapDistance >= startPoint && lapDistance <= endPoint;
            }
        }
    }
}