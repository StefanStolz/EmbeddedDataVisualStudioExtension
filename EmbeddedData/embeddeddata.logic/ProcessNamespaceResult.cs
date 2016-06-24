#region File Header

// Copyright © AWIN-Software, 2016

#endregion

#region using directives

using System;

#endregion

namespace embeddeddata.logic
{
    public class ProcessNamespaceResult
    {
        private ProcessNamespaceResult(Builder builder)
        {
            this.Namespace = builder.Namespace;
            this.SubNamespace = builder.SubNamespace;
            this.HasTestData = builder.HasTestData;
        }

        public bool HasTestData { get; }
        public string Namespace { get; }
        public string SubNamespace { get; }

        public class Builder
        {
            public bool HasTestData { get; set; }
            public string Namespace { get; set; }
            public string SubNamespace { get; set; }

            public ProcessNamespaceResult Build()
            {
                if (string.IsNullOrWhiteSpace(this.Namespace)) throw new InvalidOperationException("Namespace darf nicht leer sein.");
                if (this.SubNamespace == null) throw new InvalidOperationException("SubNamespace darf nicht null sein.");

                return new ProcessNamespaceResult(this);
            }
        }
    }
}
