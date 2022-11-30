ORG_ID=$1
echo "Using ORG_ID: $ORG_ID"
rm *.pem *.der 2>/dev/null
openssl req \
    -newkey rsa:2048 \
    -nodes \
    -keyout private_key.pem \
    -x509 \
    -days 365 \
    -out certificate.pem \
    -subj "/CN=Demo Server App ($ORG_ID)/O=SFDC/C=DK"
openssl x509 \
    -in certificate.pem \
    -pubkey \
    > public_key.pem
openssl x509 \
    -outform der \
    -in certificate.pem \
    -out certificate.der

PRIVATE_KEY=$(cat private_key.pem | base64 -)
PUBLIC_KEY=$(cat public_key.pem | base64 -)

echo "JWT_PRIVATE_KEY=$PRIVATE_KEY" >> ConsoleRunner/.env
echo "JWT_PUBLIC_KEY=$PUBLIC_KEY" >> ConsoleRunner/.env
