using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
{
    public static class FieldsLength
    {
        public static class UserId
        {
            public const int Min = 1;
            public const int Max = 450;
        }
        public static class UserName
        {
            public const int Min = 4;
            public const int Max = 25;
        }
        public static class Password
        {
            public const int Min = 6;
            public const int Max = 100;
        }
        public static class Email
        {
            public const int Min = 5;
            public const int Max = 256;
        }
        public static class MuscularGroupName
        {
            public const int Min = 4;
            public const int Max = 50;
        }
        public static class MuscleName
        {
            public const int Min = 4;
            public const int Max = 50;
        }
        public static class BodyExerciseName
        {
            public const int Min = 4;
            public const int Max = 100;
        }
    }
}
