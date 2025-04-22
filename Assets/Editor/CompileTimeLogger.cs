using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[InitializeOnLoad]
public class CompileTimeLogger
{
    static CompileTimeLogger()
    {
        CompilationPipeline.compilationStarted += OnCompilationStarted;
        CompilationPipeline.compilationFinished += OnCompilationFinished;
    }

    private static double startTime;

    private static void OnCompilationStarted(object context)
    {
        startTime = EditorApplication.timeSinceStartup;
        Debug.Log("Compilation started.");
    }

    private static void OnCompilationFinished(object context)
    {
        double compileTime = EditorApplication.timeSinceStartup - startTime;
        Debug.Log($"Compilation finished. Total time taken: {compileTime} seconds.");
    }
}
