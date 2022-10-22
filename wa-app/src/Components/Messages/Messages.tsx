import React from 'react';
import './Messages.scss';

export const Messages = () => {
    return(
        <main className="main">
        <header className="selected-contact">
            <img className="selected-contact__img" src="./assets/images/profile-icon-9.png" alt="Profile picture" />
            <h2 className="selected-contact__name">Contact Number X</h2>
        </header>
        <div className="messages">
            <div className="message message--me">
                <p className="message__content">This is my message</p>
                <span className="message__time">16:10</span>
            </div>
            <div className="message message--other">
                <p className="message__content">This is someone's message</p>
                <span className="message__time">16:08</span>
            </div>
            <div className="message message--other">
                <p className="message__content">This is someone's message that is very very long and boring</p>
                <span className="message__time">16:04</span>
            </div>
            <div className="message message--other">
                <p className="message__content">This is someone's message that is very very long and boring</p>
                <span className="message__time">16:04</span>
            </div>
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
