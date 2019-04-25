<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class Game extends Model
{
    protected $table = 'Games';
    public $primaryKey = 'name';
    public $incrementing = false;
    public $timestamps = false;


    protected $fillable = [
        'name', 'minimumAge', 'releaseDate', 'instructions', 'downloadUrl', 'storeImageUrl', 'libraryImageUrl'
    ];

    protected $casts = [
        'releaseDate' => 'date'
    ];
}
