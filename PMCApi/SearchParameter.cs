namespace Test
{
    public class SearchParameter
    {
        public string SqlOperator { get; set; }

        public SearchParameter(string fieldName, object value, string sqlOperator)
        {
            SqlOperator = sqlOperator;
            FieldName = fieldName;
            Value = value;
        }

        public string FieldName { get; set; }
        public object Value { get; set; }
    }
}