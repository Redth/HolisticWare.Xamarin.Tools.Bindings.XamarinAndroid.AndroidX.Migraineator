﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HolisticWare.Xamarin.Tools.Bindings.XamarinAndroid.AndroidX.Migraineator.Core;

namespace Sample.Migraineator.ConsoleApp
{
    class Program
    {
        static int verbosity;
        static ApiComparer api_comparer = null;

        public static void Main(string[] args)
        {
            api_comparer = new ApiComparer();

            bool show_help = false;
            List<string> names = new List<string>();
            string file_input_androidx = null;
            string file_input_android_support_28_0_0 = null;
            string file_output = null;

            Mono.Options.OptionSet option_set = new Mono.Options.OptionSet()
            {
                // ../../../../../../../X/AndroidSupportComponents-AndroidX-binderate/output/AndroidSupport.api-diff.xml
                {
                    "i|input=",
                    "input folder with Android Support Diff (Metadata.xml, Metadata*.xml)",
                    (string v) =>
                    {
                        file_input_androidx = v;
                        return;
                    }
                },
                {
                    "o|output=",
                    "output folder with AndroidX Xamarin.Android Metadata files (Metadata.xml, Metadata*.xml)",
                    (string v) =>
                    {
                        file_output = v;
                        return;
                    }
                },
                {
                    "h|help",
                    "show this message and exit",
                    v => show_help = v != null
                },
            };

            List<string> extra;
            try
            {
                extra = option_set.Parse(args);
            }
            catch (Mono.Options.OptionException e)
            {
                Console.Write("AndroidX.Migraineator: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `AndroidX.Migraineator --help' for more information.");
                return;
            }

            if (show_help)
            {
                ShowHelp(option_set);
                return;
            }

            string message = "AndroidX.Migraineator";
            if (extra.Count > 0)
            {
                message = string.Join(" ", extra.ToArray());
                Debug($"Using new message: {message}");
            }
            else
            {
                Debug($"Using default message: {message}");
            }

            if (string.IsNullOrWhiteSpace(file_output))
            {
                file_output =
                    @"../../../../../../../X/AndroidSupportComponents-AndroidX-binderate/output/AndroidX.api-info.xml"
                    ;
            }
            if ( file_input_androidx == null || string.IsNullOrWhiteSpace(file_input_androidx) )
            {
                file_input_androidx =
                    @"../../../../X/AndroidSupportComponents-AndroidX-binderate/output/AndroidSupport.api-info.xml"
                    //@"../../../../../../../X/AndroidSupportComponents-AndroidX-binderate/output/AndroidSupport.api-diff.xml"
                    ;
            }
            if (file_input_android_support_28_0_0 == null || string.IsNullOrWhiteSpace(file_input_android_support_28_0_0))
            {
                file_input_android_support_28_0_0 =
                    //@"../../../../X/AndroidSupportComponents-28.0.0-binderate/output/AndroidSupport.api-info.xml"
                    @"../../../../X/AndroidSupportComponents-AndroidX-binderate/output/AndroidSupport.api-info.previous.xml"
                    ;
            }

            Task t = ProcessApiInfoFilesAsync
                            (
                                file_input_androidx, 
                                file_input_android_support_28_0_0,
                                file_output
                            );

            Task.WaitAll(t);

           
            return;
        }

        private static async Task ProcessApiInfoFilesAsync
                                                (
                                                    string file_input_androidx,
                                                    string file_input_android_support_28_0_0,
                                                    string file_output
                                                )
        {

            string working_dir = null;

            #if NETCOREAPP && NETCOREAPP2_1
            #if DEBUG
            working_dir = "./bin/Debug/netcoreapp2.1/";
            #elif RELEASE
            working_dir = "./bin/Release/netcoreapp2.1/";
            #endif
            #else
            working_dir = "./mappings/";
            #endif

            await MappingManager.InitializeAsync(working_dir);
            await MappingManager.DumpPackageNamesAsync();

            ApiInfo api_info_old_android_support = new ApiInfo(file_input_android_support_28_0_0);
            await api_info_old_android_support.LoadAsync();
            ApiInfo api_info_new_androidx = new ApiInfo(file_input_androidx);
            await api_info_new_androidx.LoadAsync();

            await api_info_old_android_support.XmlSerializerAPI.Deserialize();
            await api_info_new_androidx.XmlSerializerAPI.Deserialize();

            api_info_old_android_support.XmlDocumentAPI.AnayseAPI();
            api_info_old_android_support.XmlDocumentAPI.DumpAPI("API.Android.Support.XmlDocumentAPI");

            api_info_new_androidx.XmlDocumentAPI.AnayseAPI();
            api_info_new_androidx.XmlDocumentAPI.DumpAPI("API.AndroidX.XmlDocumentAPI");


            api_comparer.Merge
                            (
                                ApiComparer.GoogleClassMappings,
                                api_info_old_android_support,
                                api_info_new_androidx
                            );

            Task.WaitAll();













            api_comparer.ApiInfoFileNew = file_input_androidx;
            api_comparer.ApiInfoFileOld = file_input_android_support_28_0_0;

            (
                List<string> namespaces,
                List<string> namespaces_new_suspicious,
                List<string> namespaces_old_suspicious,
                List<(string ClassName, string ClassNameFullyQualified)> classes
            ) analysis_data_old;

            //var merge1 =
            //api_info_comparer.MappingManager.Merge_Old_AndroidSupport
            //                        (
            //                            api_info_comparer.MappingManager.GoogleArtifactMappings,
            //                            api_info_comparer.ApiInfoDataOld
            //                        );
            //await api_info_comparer.MappingManager.Merge_New_AndroidSupport
            //                        (
            //                            api_info_comparer.MappingManager.GoogleArtifactMappings,
            //                            api_info_comparer.ApiInfoDataNew
            //                        );


            //analysis_data_old = api_info_comparer.Analyse(api_info_comparer.ApiInfoDataOld);

            //api_info_comparer.MappingApiInfoMatertial();






            //api_info_comparer.ModifyApiInfo
                                        //(
                                        //    api_info_comparer.ContentApiInfoNew,
                                        //    api_info_comparer.ApiInfoDataOld
                                        //);

            return;
        }

        static void ShowHelp(Mono.Options.OptionSet p)
        {
            Console.WriteLine("Usage: greet [OPTIONS]+ message");
            Console.WriteLine("Greet a list of individuals with an optional message.");
            Console.WriteLine("If no message is specified, a generic greeting is used.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);

            return;
        }

        static async Task Analyse()
        {
            /*
            (
                List<string> namespaces_28,
                List<string> namespaces_28_new_suspicious,
                List<string> namespaces_28_old_suspicious,
                List<string> classes_28
            ) = androidx_diff_comparer.Analyse(api_info_previous_androidsupport_28_0_0.api_info_previous);

            androidx_diff_comparer.DumpToFiles
            (
                api_info_previous_androidsupport_28_0_0.api_info_previous, 
                "AndroidSupport_28_0_0"
            );

            (
                string content_new,
                ApiInfo api_info_new
            )
                api_info_androidx = await androidx_diff_comparer.ApiInfo(file_input_androidx);

            (
                List<string> namespaces_x,
                List<string> namespaces_x_new_suspicious,
                List<string> namespaces_x_old_suspicious,
                List<string> classes_x
            ) = androidx_diff_comparer.Analyse(api_info_androidx.api_info_new);

            androidx_diff_comparer.DumpToFiles
            (
                api_info_androidx.api_info_new, 
                "AndroidX"
            );
            */

        }


        static void Debug(string format, params object[] args)
        {
            if (verbosity > 0)
            {
                Console.Write("# ");
                Console.WriteLine(format, args);
            }

            return;
        }
    }
}
