import './main.scss';

import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import { RecoilRoot } from 'recoil';
import { ApolloClient, ApolloProvider, InMemoryCache, from, HttpLink, ApolloLink, RequestHandler, useQuery } from '@apollo/client';
import { onError } from '@apollo/client/link/error';

const errorLink = onError((errors) => {
  if (errors.graphQLErrors) {
    errors.graphQLErrors.map((graphqlError) => {
      alert(`GraphQL error ${graphqlError.message}`);
    });
  }
});

const link = from([
  errorLink,
  new HttpLink({ uri: 'http://localhost:5000/graphql/' }),
]);

const client = new ApolloClient({
  cache: new InMemoryCache(),
  link: link
});

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <ApolloProvider client={client}>
      <RecoilRoot>
        <App />
      </RecoilRoot>
    </ApolloProvider>
  </React.StrictMode>
)
