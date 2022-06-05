# Run transactions in a json file
# There are two records in the database as a result of this command.
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --read-file ./transactions.json

# Get Record for NFT with a specific Token Id 
# Should return null since it is minted but then burned 
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --nft 0xA000000000000000000000000000000000000000


# Get Record for NFT with a specific Token Id
# wallet should be 0x3000000000000000000000000000000000000000 as it is transfered
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --nft 0xB000000000000000000000000000000000000000


# Get Record for NFT with a specific Token Id
# should exists in wallet 0x3000000000000000000000000000000000000000
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --nft 0xC000000000000000000000000000000000000000

# Get Record for NFT with a specific Token Id
# no record should be found since it is not minted yet
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --nft 0xD000000000000000000000000000000000000000

# Pass transaction information as a json file - single transaction
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --read-inline '{"Type":"Mint","TokenId":"0xD000000000000000000000000000000000000000","Address":"0x1000000000000000000000000000000000000000"}'

# Pass transaction information as a json file - multiple transactions
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --read-inline '[{"Type":"Mint","TokenId":"0x8000000000000000000000000000000000000000","Address":"0x1000000000000000000000000000000000000000"},{"Type":"Mint","TokenId":"0x9000000000000000000000000000000000000000","Address":"0x1000000000000000000000000000000000000000"}]'

# Get Record for NFT with a specific Token Id
# should belong to wallat 0x1000000000000000000000000000000000000000
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --nft 0xD000000000000000000000000000000000000000

# Get All records for a specific wallet
# should hold 2 tokens
# 0xB000000000000000000000000000000000000000 - moved to his wallet
# 0xC000000000000000000000000000000000000000 - orinally minted for this wallet
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --wallet 0x3000000000000000000000000000000000000000

# Clean up the whole database
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --reset

# Get All records for a specific wallet
# no record exists since db is nuked/reset.
dotnet ./publish/CodingChallenge.Console/CodingChallenge.Console.dll --wallet 0x3000000000000000000000000000000000000000