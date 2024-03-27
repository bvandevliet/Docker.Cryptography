# Docker.Cryptography

A container intended to run in a bridge network that provides a Web API for other containers to encrypt and decrypt sensitive data.  
The encryption method used is `aes-256-cbc`.  

### Usage

```
curl -X 'POST' \
  'http://cryptography/api/encrypt' \
  -H 'Content-Type: application/json' \
  -d '"plaintext to encrypt"'
```

```
curl -X 'POST' \
  'http://cryptography/api/decrypt' \
  -H 'Content-Type: application/json' \
  -d '"ciphertext to decrypt"'
```

Responses are in plain text. A random secret key is generated on the first request and stored in a file.  
This file is mounted as a volume so the key is persisted between container restarts.  

### Running the container

Configure the allowed hosts to improve security.  
Allowed hosts is a semicolon-delimited list of host names without port numbers.  
Requests without a matching host name will be refused.  
Host names may be prefixed with a '*.' wildcard, or use '*' to allow all hosts.  

```
# docker-compose.yml

version: '3.6'

services:
  cryptography:
    container_name: cryptography
    image: bvandevliet/cryptography:linux-arm64-v8
    restart: unless-stopped
    tty: true
    volumes:
      - ./.volumes/secrets:/app/secrets
    environment:
      - TZ=Europe/Amsterdam
      - AllowedHosts=localhost;*.local
```