//using System;
//using System.Collections.Generic;
//using System.Text;

//TODO: implement

//namespace Utility.Data
//{
//    public static class TypeFormatterUtility
//    {
//        //build sequence with parameter propagation such that each return value is the parameter for input/output of the next pass (read-only)
//        //public static AggregationTypeFormatterBuilder<TInParameters, TOutParameters> Aggregation<TInParameters, TOutParameters>(TypeFormatter<TInParameters, TOutParameters> first)
//        //{
//        //}

//        //execute sequence with the same input parameter
//        public static TypeFormatter<TInParameters, TOutParameters> Composite<TInParameters, TOutParameters>(IEnumerable<TypeFormatter<TInParameters, TOutParameters>> sequence)
//        {
//        }

//        //seek position (relative to current position, TInParameters)
//        public static TypeFormatter<TInParameters, TOutParameters> Seek()
//        {
//        }

//        //skip static amount of bytes
//        public static TypeFormatter<EmptyType, EmptyType> Skip(int bytes)
//        {
//        }

//        //skip dynamic amount of bytes
//        public static TypeFormatter<EntityCount, EntityCount> Skip()
//        {
//        }

//        //throw invalid upon encountering invalid sequence such as invalid magic numbers
//        public static TypeFormatter<> Assertion()
//        {
//        }

//        //while the condition can be read, is valid and loopCondition is applied, continue. T : output type, catch FormatException upon reading invalid entries
//        public static TypeFormatter<> While<T>(TypeFormatter conditionReader, Func<T, bool> loopCondition, TypeFormatter internReader)
//        {
//        }

//        public static TypeFormatter<> If<T>(TypeFormatter conditionReader, Func<T, bool> condition, TypeFormatter internReader)
//        {
//        }

//        public static TypeFormatter<> DoWhile<T>(TypeFormatter conditionReader, Func<T, bool> loopCondition, TypeFormatter internReader)
//        {
//        }

//        //store position, execute multiple type formatters on the same stream instance (reset position after each execution, enum that decides the final position from the set of computed positions) with different parameters
//        public static TypeFormatter<> OneToManyRelation()
//        {
//        }
//    }
//}
