﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TiDeadlock.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AppLocalization {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AppLocalization() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TiDeadlock.Resources.AppLocalization", typeof(AppLocalization).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TiDeadlock.
        /// </summary>
        public static string AppTitle {
            get {
                return ResourceManager.GetString("AppTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Патч.
        /// </summary>
        public static string MainWindowButtonPatch {
            get {
                return ResourceManager.GetString("MainWindowButtonPatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Восстановить.
        /// </summary>
        public static string MainWindowButtonRestore {
            get {
                return ResourceManager.GetString("MainWindowButtonRestore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Имена героев на английском языке.
        /// </summary>
        public static string MainWindowCheckBoxHeroes {
            get {
                return ResourceManager.GetString("MainWindowCheckBoxHeroes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Названия предметов на английском языке.
        /// </summary>
        public static string MainWindowCheckBoxItems {
            get {
                return ResourceManager.GetString("MainWindowCheckBoxItems", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Локализация.
        /// </summary>
        public static string MainWindowGroupBoxHeader {
            get {
                return ResourceManager.GetString("MainWindowGroupBoxHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Патч успешно применен....
        /// </summary>
        public static string MessageBoxDescriptionPatch {
            get {
                return ResourceManager.GetString("MessageBoxDescriptionPatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Восстановлен оригинальный файл локализации....
        /// </summary>
        public static string MessageBoxDescriptionRestore {
            get {
                return ResourceManager.GetString("MessageBoxDescriptionRestore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Некорректный путь к папке с игрой....
        /// </summary>
        public static string MessageBoxDescriptionUncorrectPath {
            get {
                return ResourceManager.GetString("MessageBoxDescriptionUncorrectPath", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ошибка.
        /// </summary>
        public static string MessageBoxErrorTitle {
            get {
                return ResourceManager.GetString("MessageBoxErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Информация.
        /// </summary>
        public static string MessageBoxInfoTitle {
            get {
                return ResourceManager.GetString("MessageBoxInfoTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Вопрос.
        /// </summary>
        public static string MessageBoxQuestionTitle {
            get {
                return ResourceManager.GetString("MessageBoxQuestionTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to В настоящее время существует проблема с обновлением игры, связанная с изменением хеш-суммы файла локализации после установки патча. Это приводит к конфликту с клиентом Steam, который не позволяет обновить игру.
        ///
        ///Режим «Сервис» предназначен для автоматического обновления файла локализации при запуске игры и восстановления исходного файла при её закрытии. Однако стоит учитывать, что этот режим добавляет программу в автозапуск и постоянно сканирует системные процессы, ожидая запуска игры. Это может снизить п [rest of string was truncated]&quot;;.
        /// </summary>
        public static string MessageBoxServiceDescription {
            get {
                return ResourceManager.GetString("MessageBoxServiceDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Сервисный режим.
        /// </summary>
        public static string MessageBoxServiceTitle {
            get {
                return ResourceManager.GetString("MessageBoxServiceTitle", resourceCulture);
            }
        }
    }
}
