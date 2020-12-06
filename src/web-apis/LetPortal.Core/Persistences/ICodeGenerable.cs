namespace LetPortal.Core.Persistences
{
    public interface ICodeGenerable
    {
        /// <summary>
        /// Use this method to generate code for CLI
        /// </summary>
        /// <returns>Code result includes namespaces, deleting code and inserting code</returns>
        CodeGenerableResult GenerateCode(string varName = null, int space = 0);
    }
}
