import React, { useRef } from 'react';
import { MessageModel } from '../../Models';
import './Messages.scss';

interface Props
{
    setMessages: React.Dispatch<React.SetStateAction<MessageModel[]>>;
    children: React.ReactNode;
};

export const Messages: React.FC<Props> = ({ children }) => {
    const textRef = useRef<HTMLTextAreaElement>(null);

    function onSendClick(event: React.MouseEvent<HTMLButtonElement>): void {
        if (textRef.current?.value.length != 0) {

        }
    }

    return(
        <main className="main">
            <header className="selected-contact">
                <img className="selected-contact__img" src="./assets/images/profile-icon-9.png" alt="Profile picture" />
                <h2 className="selected-contact__name">Contact Number X</h2>
            </header>
            <div className='messages'>
                {children}
            </div>
            <form className="message-form">
                <label className="message-form__label" htmlFor="message">Enter message:</label>
                <textarea ref={textRef} className="message-form__text" rows={6} name="message" id="message" placeholder="Enter your message..." required></textarea>
                <button onClick={onSendClick} className="message-form__submit">
                    <img className="submit__img" src="./assets/icons/send.svg" alt="Send" />
                </button>
            </form>
        </main>
    )
};
