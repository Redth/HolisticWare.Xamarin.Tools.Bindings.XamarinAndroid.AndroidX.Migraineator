﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Core.Text;
using HolisticWare.Xamarin.Tools.Bindings.XamarinAndroid.AndroidX.Migraineator.Core.Generated;

namespace HolisticWare.Xamarin.Tools.Bindings.XamarinAndroid.AndroidX.Migraineator.Core
{
    public partial class ApiInfo
    {
        public ApiInfo(string path)
        {
            api_info_path = path;

            // 3 
            this.XmlSerializerAPI = new XmlSerializerData(api_info_path);
            this.XmlDocumentAPI = new XmlDocumentData(api_info_path);
            this.LinqXDocumentAPI = new LinqXDocumentData(api_info_path);

            return;
        }

        string api_info_path;

        public string ApiInfoPath
        {
            get
            {
                return api_info_path;
            }
            protected set
            {
            }
        }

        public string ApiInfoContent
        {
            get
            {
                return api_info_content;
            }
        }

        string api_info_content = null;

        StreamReader sr = null;

        public async Task<string> LoadAsync()
        {
            sr = new StreamReader(api_info_path);
            api_info_content = await sr.ReadToEndAsync();

            return api_info_content;
        }

        public XmlSerializerData XmlSerializerAPI
        {
            get;
            private set;
        }

        public XmlDocumentData XmlDocumentAPI
        {
            get;
            private set;
        }

        public LinqXDocumentData LinqXDocumentAPI
        {
            get;
            private set;
        }
    }
}
