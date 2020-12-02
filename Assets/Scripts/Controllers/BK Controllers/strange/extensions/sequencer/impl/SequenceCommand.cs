/*
 * Copyright 2013 ThirdMotion, Inc.
 *
 *	Licensed under the Apache License, Version 2.0 (the "License");
 *	you may not use this file except in compliance with the License.
 *	You may obtain a copy of the License at
 *
 *		http://www.apache.org/licenses/LICENSE-2.0
 *
 *		Unless required by applicable law or agreed to in writing, software
 *		distributed under the License is distributed on an "AS IS" BASIS,
 *		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *		See the License for the specific language governing permissions and
 *		limitations under the License.
 */

/**
 * @class strange.extensions._sequencer.impl.SequenceCommand
 * 
 * @deprecated
 * 
 * @see strange.extensions.command.api.ICommand
 */

using strange.extensions.command.impl;
using strange.extensions.sequencer.api;

namespace strange.extensions.sequencer.impl
{
    public class SequenceCommand : Command, ISequenceCommand
    {
        private readonly ISequencer _sequencer;

        [Construct]
        public SequenceCommand(ISequencer sequencer)
        {
            _sequencer = sequencer;
        }

        public SequenceCommand()
        {
        }

        public new void Fail()
        {
            if (_sequencer != null)
            {
                _sequencer.Stop(this);
            }
        }

        public new virtual void Execute()
        {
            throw new SequencerException("You must override the Execute method in every SequenceCommand",
                SequencerExceptionType.EXECUTE_OVERRIDE);
        }

        public new void Release()
        {
            retain = false;
            if (_sequencer != null)
            {
                _sequencer.ReleaseCommand(this);
            }
        }
    }
}