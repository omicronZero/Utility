using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Transaction
{
    public class PipelineTransactionObject<TIn, TOut>
    {
        public TIn Input { get; }

        private TOut _output;
        public bool HasOutput { get; private set; }

        public TOut Output
        {
            get
            {
                if (!HasOutput)
                    throw new InvalidOperationException("Output has not been specified.");

                return _output;
            }
        }

        public void SetOutput(TOut output)
        {
            _output = output;
        }

        public PipelineTransactionObject(TIn input)
        {
            Input = input;
        }
    }
}
