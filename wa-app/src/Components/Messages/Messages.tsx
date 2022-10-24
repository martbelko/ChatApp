import React from 'react';
import './Messages.scss';
import { Message } from './Message';

export interface MessageProps
{
    authorId: number;
    content: string;
    time: Date;
}

export function Messages({ messages }: any): JSX.Element {
    console.log(messages.length);
    return(
        <main className="main">
            <header className="selected-contact">
                <img className="selected-contact__img" src="./assets/images/profile-icon-9.png" alt="Profile picture" />
                <h2 className="selected-contact__name">Contact Number X</h2>
            </header>
            <div className="messages">
                {messages.map(() => {
                    return <Message />
                })}
            </div>
            <form className="message-form">
                <label className="message-form__label" htmlFor="message">Enter message:</label>
                <textarea className="message-form__text" rows={6} name="message" id="message" placeholder="Enter your message..." required></textarea>
                <button className="message-form__submit">
                    <img className="submit__img" src="./assets/icons/send.svg" alt="Send" />
                </button>
            </form>
        </main>
    )
};
