import React from 'react';
import './Messages.scss';
import { Message } from './Message';

interface MessagesProps {
	contactName: string;
}

interface MessageModel {
	content: string;
	date: Date;
}

export const Messages: React.FC<MessagesProps> = ({ contactName }) => {
	const messages: Array<MessageModel> = [
		{
			content: 'First',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message that is very very long and boring',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message that is very very long and boring',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		},
		{
			content: 'This is someone\'s message',
			date: new Date(Date.now())
		}
	];


	return (
		<main className='main'>
			<header className='selected-contact'>
				<img className='selected-contact__img' src='./assets/images/profile-icon-9.png' alt='Profile picture' />
				<h2 className='selected-contact__name'>{contactName}</h2>
			</header>
			<div className='messages-container'>
				<div className='messages'>
					{messages.map((m, i) => <Message datetime={m.date} key={i}>{m.content}</Message>)}
				</div>
				<div className='emoji-panel'>
					<div className='emoji-panel__section'>
						<h3 className='emoji-panel__header'>Recently used</h3>
						<div className='emoji-panel__emojies'>
							<button className='emoji-button'>&#128512;</button>
							<button className='emoji-button'>&#128514;</button>
							<button className='emoji-button'>&#128513;</button>
							<button className='emoji-button'>&#128517;</button>
						</div>
					</div>
					<div className='emoji-panel__section'>
						<h3 className='emoji-panel__header'>All emojies</h3>
						<div className='emoji-panel__emojies'>
							<button className='emoji-button'>&#128512;</button>
							<button className='emoji-button'>&#128514;</button>
							<button className='emoji-button'>&#128513;</button>
							<button className='emoji-button'>&#128517;</button>
						</div>
					</div>
				</div>
				<form className='message-form'>
					<label className='message-form__label' htmlFor='emoji'>Emoji</label>
					<button className='message-form__emoji' id='emoji' type='button'>
                    E
					</button>

					<label className='message-form__label' htmlFor='file'>File</label>
					<button className='message-form__file' id='file' type='button'>
                    F
					</button>

					<label className='message-form__label' htmlFor='message'>Enter message</label>
					<textarea className='message-form__text' rows={6} name='message' id='message' placeholder='Enter your message...' required></textarea>
					<button className='message-form__submit'>
						<img className='submit__img' src='./assets/icons/send.svg' alt='Send' />
					</button>
				</form>
			</div>
		</main>
	);
};
