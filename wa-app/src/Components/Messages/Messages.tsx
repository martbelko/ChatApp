import React, { useRef } from 'react';
import { ConversationModel } from '../../Models';
import { Message } from './Message';
import './Messages.scss';

interface Props
{
    activeConversation: ConversationModel;
};

export const Messages: React.FC<Props> = ({ activeConversation }) => {
    const textRef = useRef<HTMLTextAreaElement>(null);

    function addMessage(content: string) {
        const query = gql`
          mutation {
              addMessage(input: {
              userId: ${currentUser.id},
              conversationId: ${activeConversation?.id},
              content: ${content}
              })
              {
              message {
                  id
                  content
                  time
              }, error
              }
          }
          `;
        return;
    };

    function onSendClick(e: React.MouseEvent<HTMLButtonElement>) {
        const content = textRef.current?.value;
        if (content != undefined && content.length != 0) {
            addMessage(content);
        }
    };

    return(
        <main className="main">
            <header className="selected-contact">
                <img className="selected-contact__img" src="./assets/images/profile-icon-9.png" alt="Profile picture" />
                <h2 className="selected-contact__name">Contact Number X</h2>
            </header>
            <div className='messages'>
                {activeConversation.messages.map(m => <Message authorId={m.author.id} content={m.content} datetime={m.datetime} />)}
            </div>
            <form className="message-form">
                <label className="message-form__label" htmlFor="message">Enter message:</label>
                <textarea ref={textRef} className="message-form__text" rows={6} name="message" id="message" placeholder="Enter your message..." required></textarea>
                <button onClick={onSendClick} type="button" className="message-form__submit">
                    <img className="submit__img" src="./assets/icons/send.svg" alt="Send" />
                </button>
            </form>
        </main>
    )
};
