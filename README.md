# Table of Contents

- 🚀 [Project Overview](#project-overview)
- 📋 [Requirements](#requirements)
- 📚 [Definitions](#definitions)
  - [Lemma](#lemma)
  - [Sense](#sense)
  - [Synset](#synset)
- ✨ [Features](#features)
  - [AI Comparison](#ai-comparison)
  - [Custom Lemma Adding](#custom-lemma-adding)
  - [Bookmark](#bookmark)
  - [UI Settings](#ui-settings)
    - [Language](#language)
    - [Language](#darkmode)
  - [Trending List](#trending-list)
- 🖼️ [Project Previews](#project-previews)

# Project overview
WordNet-Unity is a variant of WordNet (https://wordnet.princeton.edu) implemented using Unity and the .NET platform. It provides the core functionality of the original project, including synonym comparison and the ability to add custom words. This project has an iOS version available on the App Store (https://apps.apple.com/us/app/synonymnet/id6739968740).
# Requirement
1. **Unity**
   - Version: 2019.4 or higher
2. **.NET Standard**
   - Version: 2.0
3. **Firebase**
4. **i5 Toolkit for Unity**
   - Include the i5 Toolkit for Unity for additional functionality.
6. **Platform-Specific Requirements**
   - For iOS: Ensure compatibility with the latest iOS SDK and App Store guidelines.


# Definitions
## Lemma
Definition: A lemma is the canonical form or dictionary form of a set of words. In WordNet, a lemma represents a single word or a phrase that serves as a headword for a set of related words.
Example: For the words "run," "runs," "ran," and "running," the lemma would be "run."

## Sense
Definition: A sense refers to a specific meaning of a word as it appears in a synset. A single lemma can have multiple senses, each corresponding to a different synset.
Example: The word "bank" can have multiple senses, such as a financial institution or the side of a river, each represented by a different synset.

## Synset
Definition: A synset is a set of one or more synonyms that are interchangeable in some context. Each synset represents a single concept or meaning and is associated with a specific part of speech (noun, verb, adjective, or adverb).
Example: The synset for the concept of "car" might include the words "car," "automobile," and "motorcar."

## Connection Explanation
The connection between lemma, sense and synset can be illustrated in the following graph:
(https://en.wikipedia.org/wiki/Synonym_ring)

# Features
## AI comparsion
The project provides functionality for comparing two synonyms. As this project is the client of the whole system, you need to implement the API. The client-side logic is already implemented, making it easy to integrate AI interfaces such as OpenAI, Groq, or Gemini. A simple prompt could be: "Illustrate the difference between two words."
## Customs lemma adding
SynonymNet allows you to add custom words to the original synset.
## Bookmark 
The project includes a bookmarking feature to help users remember the words they searched for.
## Trending List
As a product focused on multiple users, SynonymNet tracks user data on search results. One important metric is trending words, representing the most frequently searched words by users.
## Settings
For users preferences, SynonymNet provides following setting:
### Language
Currently, SynonymNet supports four languages: English, Japanese, German, and Korean.
### Darkmode
The project allows users to switch between dark mode and light mode.


# Project previews
(https://en.wikipedia.org/wiki/Synonym_ring)



