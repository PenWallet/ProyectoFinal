<?php

namespace App;

use Illuminate\Database\Eloquent\Model;

class UserFriend extends Model
{
    protected $table = 'UserFriends';
    public $primaryKey = ['user1', 'user2'];
    public $incrementing = false;
    public $timestamps = false;

    protected $fillable = [
        'user1', 'user2', 'acceptedRequestDate',
    ];
}
