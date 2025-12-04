
sleep 5
node deploy.js

echo "Contract Address: "
cat ./contract.txt

dotnet backend.dll
